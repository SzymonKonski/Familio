namespace Application.Hubs;

public static class ConfigConnections
{
    public static readonly List<(string UserId, string GoroupId, string ConncetionId)> ConnectionsGroupsMap = new();
    public static readonly List<(string UserId, string ConncetionId)> ConnectionsMap = new();
}