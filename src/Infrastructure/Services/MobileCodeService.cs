using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class MobileCodeService : IMobileCodeService
{
    private readonly IApplicationDbContext _context;

    public MobileCodeService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> VerifyCode(string userId, AuthActionType actionType, string code)
    {
        var res = await _context.MobileCodes.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.Type == actionType && x.Code == code);

        if (res == null)
            return string.Empty;

        var token = res.Token;

        _context.MobileCodes.Remove(res);
        await _context.SaveChangesAsync(CancellationToken.None);

        return token;
    }

    public async Task<string> GenerateCode(string userId, AuthActionType actionType, string token)
    {
        var code = RandomString(4);
        var res = await _context.MobileCodes.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.Type == actionType);

        if (res != null)
        {
            res.Code = code;
            res.Token = token;
            _context.MobileCodes.Update(res);
        }
        else
        {
            await _context.MobileCodes.AddAsync(new MobileCode
            {
                Token = token,
                Code = code,
                Type = actionType,
                UserId = userId
            });
        }

        await _context.SaveChangesAsync(CancellationToken.None);

        return code;
    }

    public static string RandomString(int length)
    {
        var random = new Random();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}