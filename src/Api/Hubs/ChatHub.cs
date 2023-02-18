using Application.Common.Interfaces;
using Application.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IApplicationDbContext _context;
    private readonly IUserManagerService _userManagerService;

    public ChatHub(IApplicationDbContext context, IUserManagerService userManagerService)
    {
        _context = context;
        _userManagerService = userManagerService;
    }

    public async Task NewMessage(string groupId, MessageViewModel createdMessage)
    {
        await Clients.Group(groupId).SendAsync("newMessage", createdMessage);
    }

    public async Task AddUser(string groupId)
    {
        await Clients.Group(groupId).SendAsync("addUser");
    }

    public async Task RemoveUser(string groupId)
    {
        await Clients.Group(groupId).SendAsync("removeUser");
    }

    public async Task Join(string groupId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (userId == null)
                throw new UnauthorizedAccessException("UserId is null");

            if (ConfigConnections.ConnectionsGroupsMap.Any(x => x.UserId == userId && x.GoroupId == groupId)) return;

            if (!_context.DomainUserGroups.Any(
                    x => x.DomainUserId == userId && x.GroupId == groupId)) return;

            // Join to new chat room
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

            ConfigConnections.ConnectionsGroupsMap.Add((userId, groupId, Context.ConnectionId));

            // Tell others to update their list of users
            var user = new UserViewModel
            {
                GroupId = groupId,
                UserId = userId
            };

            await Clients.OthersInGroup(groupId).SendAsync("addUser", user);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
        }
    }

    public async Task StartSharingLocalization(string groupId)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (userId == null)
                throw new UnauthorizedAccessException("UserId is null");

            var userConnectionsInGroup = ConfigConnections.ConnectionsGroupsMap.Where(x =>  x.GoroupId == groupId && x.UserId == userId && x.ConncetionId != Context.ConnectionId).ToList();
            var connectionids = userConnectionsInGroup.Select(x => x.ConncetionId).ToList();
            IReadOnlyList<string> list = new List<string>(connectionids);

            await Clients.Clients(list).SendAsync("stopSharingLocalization", groupId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (userId == null)
                throw new UnauthorizedAccessException("UserId is null");
            var user = await _userManagerService.GetUserById(userId);

            ConfigConnections.ConnectionsMap.Add((userId, Context.ConnectionId));
            var groups = _context.DomainUserGroups.Where(x => x.DomainUserId == userId).ToList();

            foreach (var domainUserGroup in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, domainUserGroup.GroupId);
                ConfigConnections.ConnectionsGroupsMap.Add((user.Id, domainUserGroup.GroupId, Context.ConnectionId));
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (userId == null)
                throw new UnauthorizedAccessException("UserId is null");
            ConfigConnections.ConnectionsMap.RemoveAll(x => x.ConncetionId == Context.ConnectionId);
            ConfigConnections.ConnectionsGroupsMap.RemoveAll(x => x.ConncetionId == Context.ConnectionId);

            var groups = _context.DomainUserGroups.Where(x => x.DomainUserId == userId).ToList();
            foreach (var domainUserGroup in groups)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, domainUserGroup.GroupId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
        }

        await base.OnDisconnectedAsync(exception);
    }
}