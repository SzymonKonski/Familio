namespace Application.Hubs;

public interface INotificationService
{
    Task NewMessage(string groupId, MessageViewModel createdMessage);

    //Task RemoveChatMessage(int messageId);

    Task AddChatRoom(string groupId, string userId);

    //Task RemoveChatRoom(string groupId);

    //Task OnRoomDeleted(string groupId);

    Task AddUser(string groupId, string userId);

    Task RemoveUser(string groupId, string userId);
}