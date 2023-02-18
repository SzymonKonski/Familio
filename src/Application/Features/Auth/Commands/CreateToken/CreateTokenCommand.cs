using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using MediatR;

namespace Application.Features.Auth.Commands.CreateToken;

public class CreateTokenCommand : IRequest<AuthResult>
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
}

public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, AuthResult>
{
    private readonly ITokenService _tokenService;

    public CreateTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<AuthResult> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.CreateTokenForVerifiedUser(request.Email, request.Password, cancellationToken);
    }
}