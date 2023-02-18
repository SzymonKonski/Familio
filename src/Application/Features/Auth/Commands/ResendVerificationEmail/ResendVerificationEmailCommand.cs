using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.ResendVerificationEmail;

public class ResendVerificationEmailCommand : IRequest<Result>
{
    public string Email { get; set; } = default!;
}

public class ResendVerificationEmailCommandHandler : IRequestHandler<ResendVerificationEmailCommand, Result>
{
    private readonly IEmailCallbackService _emailCallbackService;
    private readonly UserManager<DomainUser> _userManager;

    public ResendVerificationEmailCommandHandler(UserManager<DomainUser> userManager,
        IEmailCallbackService emailCallbackService)
    {
        _userManager = userManager;
        _emailCallbackService = emailCallbackService;
    }

    public async Task<Result> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result.Failure(new[] {"Could not find user!"});

        var result = await _emailCallbackService.SendConfirmationEmail(user);

        if (!result)
            throw new EmailNotSendException("Email was not send.");

        return Result.Success();
    }
}