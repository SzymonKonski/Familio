namespace Application.Features.LocationSharing;

public static class LocalizationDictionary
{
    public static readonly Dictionary<(string GroupId, string UserId), Localization> Locations = new();
}

public class Localization
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Error { get; set; }
    public DateTime TimeStamp { get; set; }
}