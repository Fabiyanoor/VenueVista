using Microsoft.EntityFrameworkCore;
using MyApp.Web.Data.Entities.Venues;
using MyApp.Web.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Data.Entities.Package;

public class VenuePackage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public PackageTier Tier { get; set; }

    [Required]
    public int BaseCapacity { get; set; }

    [Required]
    public int BaseDurationHours { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal BasePrice { get; set; }

    // Extension pricing (for customization)
    [Precision(18, 2)]
    public decimal PricePerAdditionalPerson { get; set; }

    [Precision(18, 2)]
    public decimal PricePerAdditionalHour { get; set; }

    // Package includes
    public bool IncludesDecoration { get; set; }
    public bool IncludesCake { get; set; }
    public bool IncludesSoundSystem { get; set; }

    [MaxLength(1000)]
    public string? IncludedServicesDescription { get; set; }

    // Relationships
    [Required]
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }

    public ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();


    public DateTime UpdatedAt { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class PackageService
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    public Guid PackageId { get; set; }
    public VenuePackage? Package { get; set; }

    public bool IsIncludedInPackage { get; set; }
    public bool IsAvailableForCustomization { get; set; } = true;
}