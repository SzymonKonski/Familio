using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.Register;

public record RegisterCommand : IRequest<Result>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
    public string Firstname { get; set; } = default!;
    public string Surname { get; set; } = default!;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailCallbackService _emailCallbackService;
    private readonly UserManager<DomainUser> _userManager;

    public RegisterCommandHandler(UserManager<DomainUser> userManager, IApplicationDbContext dbContext,
        IEmailCallbackService emailCallbackService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _emailCallbackService = emailCallbackService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new DomainUser
        {
            Email = request.Email,
            Firstname = request.Firstname,
            Surname = request.Surname,
            UserName = request.Email,
            DomainUsername = request.Firstname + ' ' + request.Surname
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var emailResult = await _emailCallbackService.SendConfirmationEmail(user);

            if (!emailResult)
                throw new EmailNotSendException("Email was not send.");

            await _dbContext.SaveChangesAsync(cancellationToken);

            var res = Result.Success();
            res.SuccessParam = user.Id;
            return res;
        }

        return Result.Failure(result.Errors.Select(x => x.Description));
    }
}