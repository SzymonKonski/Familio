using Application.Common.Interfaces;
using Domain.Entities;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger _logger;
    private readonly IUserManagerService _userManagerService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService,
        IUserManagerService userManagerService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _userManagerService = userManagerService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? string.Empty;
        DomainUser? user = null;

        if (!string.IsNullOrEmpty(userId)) user = await _userManagerService.GetUserById(userId);

        _logger.LogInformation("Familio Request: {Name} {@UserId} {@UserEmail} {@Request}",
            requestName, userId, user?.Email, request);
    }
}