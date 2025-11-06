using MyApp.Web.Data.Entities.Bookings;
using MyApp.Web.Data.Entities.Package;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Data.Entities.Venues;

public class Venue
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [Required]
    public string? Address { get; set; }

    public ICollection<VenueImage> Images { get; set; } = new List<VenueImage>();

    [Required]
    [MaxLength(50)]
    public string? Type { get; set; } // Indoor / Outdoor / Both

    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } // Available / Booked / Maintenance

    [Range(0, 5)]
    public double Rating { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Booking> Bookings { get; set; } = new List<Booking>();

    public DateTime UpdatedAt { get; set; }

    public ICollection<VenuePackage> Packages { get; set; } = new List<VenuePackage>();

    public ICollection<AdditionalService> AdditionalServices { get; set; } = new List<AdditionalService>();
}