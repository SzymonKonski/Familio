namespace Infrastructure.Services.AppSettings;

public class EmailSettings
{
    public string? To { get; set; }
    public string? From { get; set; }
    public string? DisplayName { get; set; }
    public string? SendGridApiKey { get; set; }
}