using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using Infrastructure.Services.AppSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly JwtSecurityTokenSettings _jwt;
    private readonly TokenValidationParameters _refreshTokenValidationParams;
    private readonly UserManager<DomainUser> _userManager;

    public TokenService(IOptions<JwtSecurityTokenSettings> jwt, TokenValidationParameters refreshTokenValidationParams,
        UserManager<DomainUser> userManager, IApplicationDbContext dbContext)
    {
        _jwt = jwt.Value;
        _refreshTokenValidationParams = refreshTokenValidationParams;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<AuthResult> CreateTokenForVerifiedUser(string email, string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return new AuthResult
            {
                Success = false,
                Errors = new List<string> {"Invalid credentials."}
            };

        // Only allow login if email is confirmed
        if (!user.EmailConfirmed)
            return new AuthResult
            {
                Success = false,
                Errors = new List<string> {"Invalid credentials."}
            };

        // Used as user lock
        if (user.LockoutEnabled)
            return new AuthResult
            {
                Success = false,
                Errors = new List<string> {"This account has been locked."}
            };

        if (await _userManager.CheckPasswordAsync(user, password))
            return await GenerateTokenForVerifiedUser(cancellationToken, user);

        return new AuthResult
        {
            Success = false,
            Errors = new List<string> {"Invalid login attempt."}
        };
    }


    public async Task<AuthResult> RefreshToken(string accessToken, string refreshToken,
        CancellationToken cancellationToken)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // This validation function will make sure that the token meets the validation parameters
            // and its an actual jwt token not just a random string
            var principal =
                jwtTokenHandler.ValidateToken(accessToken, _refreshTokenValidationParams, out var validatedToken);

            // Now we need to check if the token has a valid security algorithm
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == false) return null;
            }

            // Will get the time stamp in unix time
            var utcExpiryDate =
                long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value);

            // we convert the expiry date from seconds to the date
            var expDate = UnixTimeStampToDateTime(utcExpiryDate);

            if (expDate > DateTime.UtcNow)
                return new AuthResult
                {
                    Errors = new List<string> {"We cannot refresh this since the token has not expired"},
                    Success = false
                };

            // Check the token we got if its saved in the db
            var storedRefreshToken =
                await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

            if (storedRefreshToken == null)
                return new AuthResult
                {
                    Errors = new List<string> {"refresh token does not exist"},
                    Success = false
                };

            // Check the date of the saved token if it has expired
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthResult
                {
                    Errors = new List<string> {"token has expired, user needs to relogin"},
                    Success = false
                };

            // check if the refresh token has been used
            if (storedRefreshToken.IsUsed)
                return new AuthResult
                {
                    Errors = new List<string> {"token has been used"},
                    Success = false
                };

            // Check if the token is revoked
            if (storedRefreshToken.IsRevoked)
                return new AuthResult
                {
                    Errors = new List<string> {"token has been revoked"},
                    Success = false
                };

            // we are getting here the jwt token id
            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // check the id that the recieved token has against the id saved in the db
            if (storedRefreshToken.JwtId != jti)
                return new AuthResult
                {
                    Errors = new List<string> {"the token does not matched the saved token"},
                    Success = false
                };

            storedRefreshToken.IsUsed = true;
            _dbContext.RefreshTokens.Update(storedRefreshToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            return await GenerateTokenForVerifiedUser(cancellationToken, dbUser);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private async Task<AuthResult> GenerateTokenForVerifiedUser(CancellationToken cancellationToken, DomainUser user)
    {
        var jwtSecurityToken = await CreateJwtToken(user);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var refreshToken = new RefreshToken
        {
            JwtId = jwtSecurityToken.Id,
            IsUsed = false,
            UserId = user.Id,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            IsRevoked = false,
            Token = RandomString(25) + Guid.NewGuid()
        };

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResult
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token,
            Success = true
        };
    }

    private async Task<JwtSecurityToken> CreateJwtToken(DomainUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var rolesInGroups = _dbContext.DomainUserGroups.Where(x => x.DomainUserId == user.Id).ToList();

        //var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

        var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            }
            .Union(userClaims);
        //.Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private string RandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dtDateTime;
    }
}