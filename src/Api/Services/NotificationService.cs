using Api.Hubs;
using Application.Common.Interfaces;
using Application.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;


    public NotificationService(IHubContext<ChatHub> hubContext, IApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    public async Task RemoveUser(string groupId, string userId)
    {
        var removedUser = new UserViewModel
        {
            GroupId = groupId,
            UserId = userId
        };

        await _hubContext.Clients.Group(groupId).SendAsync("removeUser", removedUser);
    }

    public async Task AddChatRoom(string groupId, string userId)
    {
        var items =
            ConfigConnections.ConnectionsMap.Where(x => x.UserId == userId).ToList();

        foreach (var item in items)
        {
            await _hubContext.Groups.AddToGroupAsync(item.ConncetionId, groupId);
            ConfigConnections.ConnectionsGroupsMap.Add((userId, groupId, item.ConncetionId));
        }
    }

    public async Task NewMessage(string groupId, MessageViewModel createdMessage)
    {
        await _hubContext.Clients.Group(groupId).SendAsync("newMessage", createdMessage);
    }

    public async Task AddUser(string groupId, string userId)
    {
        try
        {
            if (userId == null)
                throw new UnauthorizedAccessException("UserId is null");

            var items =
                ConfigConnections.ConnectionsMap.Where(x => x.UserId == userId).ToList();

            foreach (var item in items)
            {
                if (!_context.DomainUserGroups.Any(
                        x => x.DomainUserId == userId && x.GroupId == groupId))
                    throw new UnauthorizedAccessException("User is not in group");

                // Join to new chat room
                await _hubContext.Groups.AddToGroupAsync(item.ConncetionId, groupId);

                ConfigConnections.ConnectionsGroupsMap.Add((userId, groupId, item.ConncetionId));

                // Tell others to update their list of users
            }

            var user = new UserViewModel
            {
                GroupId = groupId,
                UserId = userId
            };

            var connectionids = items.Select(x => x.ConncetionId).ToList();
            IReadOnlyList<string> list = new List<string>(connectionids);
            await _hubContext.Clients.GroupExcept(groupId, list).SendAsync("addUser",
                user);
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.User(userId).SendAsync("onError",
                "You failed to join the chat room!" + ex.Message);
        }
    }

    public async Task OnRoomDeleted(string groupId)
    {
        await _hubContext.Clients.Group(groupId).SendAsync("onRoomDeleted",
            $"Room {groupId} has been deleted.\nYou are moved to the first available room!");
    }

    public async Task RemoveChatMessage(int messageId)
    {
        await _hubContext.Clients.All.SendAsync("removeChatMessage", messageId);
    }

    public async Task RemoveChatRoom(string roomId)
    {
        await _hubContext.Clients.All.SendAsync("removeChatRoom", roomId);
    }
}