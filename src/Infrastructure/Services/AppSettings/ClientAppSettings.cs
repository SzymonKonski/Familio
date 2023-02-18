namespace Infrastructure.Services.AppSettings;

public class ClientAppSettings
{
    public string? Url { get; set; }
    public string? EmailConfirmationPath { get; set; }
    public string? ResetPasswordPath { get; set; }
    public string? AddUserToGroupPath { get; set; }
}