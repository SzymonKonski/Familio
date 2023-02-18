using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string UserId { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string ConfirmPassword { get; set; } = default!;

    public string Code { get; set; } = default!;
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IMobileCodeService _mobileCodeService;
    private readonly UserManager<DomainUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<DomainUser> userManager, IMobileCodeService mobileCodeService)
    {
        _userManager = userManager;
        _mobileCodeService = mobileCodeService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return Result.Failure(new[] {"Invalid credentials."});

        var token = await _mobileCodeService.VerifyCode(user.Id, AuthActionType.ResetPassword, request.Code);
        if (token == string.Empty)
            return Result.Failure(new[] {"Invalid token"});

        var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(x => x.Description));
    }
}