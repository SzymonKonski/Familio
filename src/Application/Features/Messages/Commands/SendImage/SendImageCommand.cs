using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Hubs;
using Application.Security;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Application.Features.Messages.Commands.SendImage;

[Authorize(Roles = "Parent,Child,Relative")]
public record SendImageCommand : IRequest<int>
{
    public string GroupId { get; set; } = default!;

    public IFormFile File { get; set; }
}

public class SendImageCommandHandler : IRequestHandler<SendImageCommand, int>
{
    private readonly IAzureStorage _azureStorage;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileValidator _fileValidator;
    private readonly INotificationService _notificationService;
    private readonly UserManager<DomainUser> _userManager;


    public SendImageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        UserManager<DomainUser> userManager, INotificationService notificationService,
        IFileValidator fileValidator, IAzureStorage azureStorage)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
        _notificationService = notificationService;
        _azureStorage = azureStorage;
        _fileValidator = fileValidator;
    }

    public async Task<int> Handle(SendImageCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;

        if (!_fileValidator.IsValid(file))
            throw new ValidationException("File is not valid");

        var user = await _userManager.Users.FirstAsync(u => u.Id == _currentUserService.UserId, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(DomainUser), _currentUserService.UserId);

        var group = _context.Groups.FirstOrDefault(r => r.Id == command.GroupId);
        if (group == null) throw new NotFoundException(nameof(Group), command.GroupId);

        var domainGroup =
            await _context.DomainUserGroups.FirstOrDefaultAsync(x => x.DomainUserId == user.Id && x.GroupId == group.Id,
                cancellationToken);
        if (domainGroup == null) throw new NotFoundException(nameof(DomainUserGroup));

        var fileUrl = await _azureStorage.UploadChatImageAsync(file, group.Id, Guid.NewGuid().ToString());

        var message = new Message
        {
            ImageContent = fileUrl,
            Content = null,
            Timestamp = DateTime.UtcNow,
            GroupId = domainGroup.GroupId,
            CreatedByUser = domainGroup,
            UserId = domainGroup.DomainUserId
        };

        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var messageViewModel = new MessageViewModel
        {
            Id = message.Id,
            ImageContent = message.ImageContent,
            Content = message.Content,
            UserId = domainGroup.DomainUserId,
            Timestamp = message.Timestamp,
            GroupId = domainGroup.GroupId,
            UserAvatar = user.Avatar,
            UserName = domainGroup.UserName
        };

        await _notificationService.NewMessage(group.Id, messageViewModel);
        return messageViewModel.Id;
    }
}