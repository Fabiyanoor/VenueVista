using Microsoft.EntityFrameworkCore;
using MyApp.Web.Data.Entities.Bookings;
using MyApp.Web.Data.Entities.Package;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Web.Data.Entities.Venues;

public class BookingAdditionalService
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }

    public Guid AdditionalServiceId { get; set; }
    public AdditionalService? AdditionalService { get; set; }

    [Precision(18, 2)]
    public decimal PriceAtBooking { get; set; }
    public int Quantity { get; set; } = 1;
}

public class BookingPackageService
{
    [Key]
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }
    public Booking? Booking { get; set; }

    public Guid PackageServiceId { get; set; }
    public PackageService? PackageService { get; set; }

    [Precision(18, 2)]
    public decimal PriceAtBooking { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsIncludedInPackage { get; set; }
}