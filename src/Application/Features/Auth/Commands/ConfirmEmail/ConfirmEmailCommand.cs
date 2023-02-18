using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommand : IRequest<Result>
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Code { get; set; } = default!;
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IMobileCodeService _mobileCodeService;
    private readonly UserManager<DomainUser> _userManager;

    public ConfirmEmailCommandHandler(UserManager<DomainUser> userManager, IMobileCodeService mobileCodeService)
    {
        _userManager = userManager;
        _mobileCodeService = mobileCodeService;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        DomainUser user;

        if (!string.IsNullOrEmpty(request.UserId))
            user = await _userManager.FindByIdAsync(request.UserId);
        else
            user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result.Failure(new[] {"Could not find user!"});

        var res = await _mobileCodeService.VerifyCode(user.Id, AuthActionType.ConfirmEmail, request.Code);
        if (res == string.Empty)
            return Result.Failure(new[] {"Invalid"});

        var token = res ?? throw new ArgumentNullException(nameof(res));
        var result = await _userManager.ConfirmEmailAsync(user, token);

        return result.Succeeded ? Result.Success() : Result.Failure(result.Errors.Select(x => x.Description));
    }
}