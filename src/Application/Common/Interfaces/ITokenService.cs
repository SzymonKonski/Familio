using Application.Features.Auth.Common;

namespace Application.Common.Interfaces;

public interface ITokenService
{
    Task<AuthResult> CreateTokenForVerifiedUser(string email, string password, CancellationToken cancellationToken);

    Task<AuthResult> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken);
}