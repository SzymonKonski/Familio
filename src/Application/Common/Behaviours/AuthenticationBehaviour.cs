using System.Reflection;
using Application.Common.Interfaces;
using Application.Security;
using MediatR;

namespace Application.Common.Behaviours;

public class AuthenticationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public AuthenticationBehaviour(
        ICurrentUserService currentUserService,
        IIdentityService identityService, IApplicationDbContext context)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var authenticateAttribute = request.GetType().GetCustomAttributes<AuthenticateAttribute>().ToList();

        if (authenticateAttribute.Any())
        {
            if (_currentUserService.UserId == null) throw new UnauthorizedAccessException();

            return await next();
        }

        return await next();
    }
}