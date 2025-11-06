using Microsoft.EntityFrameworkCore;
using MyApp.Web.Data.Entities.Package;
using MyApp.Web.Data.Entities.Venues;
using MyApp.Web.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Data.Entities.Bookings;

public class Booking
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }

    // Package booking
    public Guid? PackageId { get; set; }
    public VenuePackage? Package { get; set; }

    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    // Customization Options
    public bool IsCustomPackage { get; set; } = false;
    public string? CustomPackageName { get; set; }

    // Modified capacity/duration (if different from package defaults)
    public int? ModifiedCapacity { get; set; }
    public int? ModifiedDurationHours { get; set; }

    // Pricing Breakdown
    [Precision(18, 2)]
    public decimal BasePackageCost { get; set; }

    [Precision(18, 2)]
    public decimal CapacityExtensionCost { get; set; }

    [Precision(18, 2)]
    public decimal DurationExtensionCost { get; set; }

    [Precision(18, 2)]
    public decimal AdditionalServicesCost { get; set; }

    [Precision(18, 2)]
    public decimal TotalCost { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } // Pending, Confirmed, Canceled

    // Relationships
    public ICollection<BookingAdditionalService> BookingAdditionalServices { get; set; } = new List<BookingAdditionalService>();
    public ICollection<BookingPackageService> BookingPackageServices { get; set; } = new List<BookingPackageService>();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}