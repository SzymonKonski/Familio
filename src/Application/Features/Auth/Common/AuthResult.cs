namespace Application.Features.Auth.Common;

public class AuthResult
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public List<string> Errors { get; set; }
}