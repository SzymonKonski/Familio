namespace Application.Features.LocationSharing.Queries.GetUsersLocation;

public class UserLocalizationDto
{
    public string UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Error { get; set; }
    public DateTime TimeStamp { get; set; }
}