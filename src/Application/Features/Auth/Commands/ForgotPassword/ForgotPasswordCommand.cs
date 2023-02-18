using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result>
{
    public string Email { get; set; } = default!;
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IEmailCallbackService _emailCallbackService;
    private readonly IMobileCodeService _mobileCodeService;
    private readonly UserManager<DomainUser> _userManager;

    public ForgotPasswordCommandHandler(UserManager<DomainUser> userManager, IEmailCallbackService emailCallbackService,
        IMobileCodeService mobileCodeService)
    {
        _userManager = userManager;
        _emailCallbackService = emailCallbackService;
        _mobileCodeService = mobileCodeService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure(new[] {"Please verify your email address."});

        var result = await _emailCallbackService.SendResetPasswordLink(user);

        if (!result)
            throw new EmailNotSendException("Email was not send.");

        var res = Result.Success();
        res.SuccessParam = user.Id;
        return res;
    }
}