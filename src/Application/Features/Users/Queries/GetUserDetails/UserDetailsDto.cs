namespace Application.Features.Users.Queries.GetUserDetails;

public class UserDetailsDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string? AvatarUrl { get; set; }
}