using Application.Common.Interfaces;
using Application.Features.Auth.Common;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResult>
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _tokenService.RefreshToken(request.AccessToken, request.RefreshToken, cancellationToken);
    }
}