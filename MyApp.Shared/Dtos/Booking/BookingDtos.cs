using MyApp.Shared.Dtos.Venue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.Booking
{
    public class CreateBookingDto
    {
        public Guid VenueId { get; set; }
        public Guid? PackageId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public double Duration { get; set; }
        public bool IsCustomPackage { get; set; } = false;
        public string? CustomPackageName { get; set; }
        public int? ModifiedCapacity { get; set; }
        public int? ModifiedDurationHours { get; set; }
        public List<Guid> SelectedAdditionalServiceIds { get; set; } = new List<Guid>();
        public List<Guid> SelectedPackageServiceIds { get; set; } = new List<Guid>();
    }

    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public Guid? PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCustomPackage { get; set; }
        public string? CustomPackageName { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class BookingWithUserDto
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;
        public Guid? PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCustomPackage { get; set; }
        public string? CustomPackageName { get; set; }
        public int? ModifiedCapacity { get; set; }
        public int? ModifiedDurationHours { get; set; }
        public decimal BasePackageCost { get; set; }
        public decimal CapacityExtensionCost { get; set; }
        public decimal DurationExtensionCost { get; set; }
        public decimal AdditionalServicesCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AdditionalServiceDto> AdditionalServices { get; set; } = new List<AdditionalServiceDto>();
        public List<PackageServiceDto> PackageServices { get; set; } = new List<PackageServiceDto>();
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class VenueAvailabilityDto
    {
        public bool IsAvailable { get; set; }
        public List<DateTime> BookedDates { get; set; } = new List<DateTime>();
    }
}