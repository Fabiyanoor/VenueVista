using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.Venue
{
    public class VenuePackageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PackageTier Tier { get; set; }
        public int BaseCapacity { get; set; }
        public int BaseDurationHours { get; set; }
        public decimal BasePrice { get; set; }
        public decimal PricePerAdditionalPerson { get; set; }
        public decimal PricePerAdditionalHour { get; set; }
        public bool IncludesDecoration { get; set; }
        public bool IncludesCake { get; set; }
        public bool IncludesSoundSystem { get; set; }
        public string IncludedServicesDescription { get; set; } = string.Empty;
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public List<PackageServiceDto> PackageServices { get; set; } = new List<PackageServiceDto>();
    }

    public class CreateVenuePackageDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public PackageTier Tier { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BaseCapacity { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BaseDurationHours { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerAdditionalPerson { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal PricePerAdditionalHour { get; set; } = 0;

        public bool IncludesDecoration { get; set; }
        public bool IncludesCake { get; set; }
        public bool IncludesSoundSystem { get; set; }

        [MaxLength(1000)]
        public string? IncludedServicesDescription { get; set; }

        [Required]
        public Guid VenueId { get; set; }

        public List<CreatePackageServiceDto> PackageServices { get; set; } = new List<CreatePackageServiceDto>();
    }

    public class PackageServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsIncludedInPackage { get; set; }
        public bool IsAvailableForCustomization { get; set; }
    }

    public class CreatePackageServiceDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsIncludedInPackage { get; set; }
        public bool IsAvailableForCustomization { get; set; } = true;
    }

    public enum PackageTier
    {
        Basic = 1,
        Intermediate = 2,
        Advance = 3,
        Custom = 4
    }
}