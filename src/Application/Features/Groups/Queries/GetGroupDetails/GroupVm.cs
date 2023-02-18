namespace Application.Features.Groups.Queries.GetGroupDetails;

public class GroupVm
{
    public string GroupId { get; set; }

    public string Name { get; set; }

    public IList<UserDto> Users { get; set; } = new List<UserDto>();
}