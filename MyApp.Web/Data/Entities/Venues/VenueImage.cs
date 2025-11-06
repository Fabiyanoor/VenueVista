

namespace MyApp.Web.Data.Entities.Venues;


public class VenueImage
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }
}
