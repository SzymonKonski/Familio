using System.Reflection;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
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
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Any())
        {
            if (_currentUserService.UserId == null) throw new UnauthorizedAccessException();

            var groupPropertyInfo = request.GetType().GetProperty("GroupId");
            string? groupId = null;

            if (groupPropertyInfo != null)
            {
                groupId = (string?) groupPropertyInfo.GetValue(request);
                if (string.IsNullOrEmpty(groupId))
                    throw new ForbiddenAccessException("GroupId cannot be empty");
            }

            if (groupId == null) return await next();

            if (await _context.Groups.FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken) == null)
                throw new NotFoundException(nameof(Group), groupId);

            var authorizeAttributesWithRoles =
                authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles)).ToList();

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                foreach (var role in roles)
                {
                    var isInRole =
                        await _identityService.IsInRoleAsync(_currentUserService.UserId, role.Trim(), groupId);
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }

                if (!authorized) throw new ForbiddenAccessException("You do not have access to this endpoint");
            }

            var authorizeAttributesWithPolicies =
                authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy)).ToList();
            if (authorizeAttributesWithPolicies.Any())
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(_currentUserService.UserId, policy);

                    if (!authorized) throw new ForbiddenAccessException();
                }
        }

        return await next();
    }
}