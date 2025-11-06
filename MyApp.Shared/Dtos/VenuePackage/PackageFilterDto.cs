using MyApp.Shared.Dtos.Venue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.VenuePackage;

// Update your existing DTOs or create new ones
public class PackageFilterDto
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public List<PackageTier>? Tiers { get; set; }
    public List<string>? VenueTypes { get; set; } // Changed from Guid to string
    public string? SearchTerm { get; set; }
}

public class PackageFilterOptionsDto
{
    public PriceRangeDto PriceRange { get; set; } = new();
    public CapacityRangeDto CapacityRange { get; set; } = new();
    public List<PackageTierOptionDto> Tiers { get; set; } = new();
    public List<VenueTypeOptionDto> VenueTypes { get; set; } = new();
}

public class PriceRangeDto
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}

public class CapacityRangeDto
{
    public int Min { get; set; }
    public int Max { get; set; }
}

public class PackageTierOptionDto
{
    public PackageTier Tier { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class VenueTypeOptionDto
{
    public string Name { get; set; } = string.Empty; // Only name since Type is string
    public int Count { get; set; }
}
