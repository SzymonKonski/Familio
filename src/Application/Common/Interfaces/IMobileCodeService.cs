using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IMobileCodeService
{
    Task<string> GenerateCode(string userId, AuthActionType actionType, string token);

    Task<string> VerifyCode(string userId, AuthActionType actionType, string code);
}