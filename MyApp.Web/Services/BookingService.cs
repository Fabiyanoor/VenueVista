using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities.Bookings;
using MyApp.Web.Data.Entities.Package;
using MyApp.Web.Data.Entities.Venues;
using MyApp.Web.Data.Enum;

namespace MyApp.Web.Services
{
    public class BookingService : IBookingService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public BookingService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MethodResult<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                // Validate venue
                var venue = await context.Venues
                    .Include(v => v.AdditionalServices)
                    .FirstOrDefaultAsync(v => v.Id == bookingDto.VenueId);
                if (venue == null)
                    return MethodResult<BookingDto>.Fail("Venue not found.");
                if (venue.Status == "Maintenance")
                    return MethodResult<BookingDto>.Fail("Venue is under maintenance.");

                VenuePackage? selectedPackage = null;
                decimal basePackageCost = 0;
                decimal capacityExtensionCost = 0;
                decimal durationExtensionCost = 0;

                // Handle package booking
                if (bookingDto.PackageId.HasValue)
                {
                    selectedPackage = await context.VenuePackages
                        .Include(p => p.PackageServices)
                        .FirstOrDefaultAsync(p => p.Id == bookingDto.PackageId.Value && p.IsActive);

                    if (selectedPackage == null)
                        return MethodResult<BookingDto>.Fail("Selected package not found.");

                    basePackageCost = selectedPackage.BasePrice;

                    // Calculate capacity extension cost
                    if (bookingDto.ModifiedCapacity.HasValue && bookingDto.ModifiedCapacity > selectedPackage.BaseCapacity)
                    {
                        var additionalPeople = bookingDto.ModifiedCapacity.Value - selectedPackage.BaseCapacity;
                        capacityExtensionCost = additionalPeople * selectedPackage.PricePerAdditionalPerson;
                    }

                    // Calculate duration extension cost
                    if (bookingDto.ModifiedDurationHours.HasValue && bookingDto.ModifiedDurationHours > selectedPackage.BaseDurationHours)
                    {
                        var additionalHours = bookingDto.ModifiedDurationHours.Value - selectedPackage.BaseDurationHours;
                        durationExtensionCost = additionalHours * selectedPackage.PricePerAdditionalHour;
                    }
                }

                // Validate additional services
                var selectedServiceIds = bookingDto.SelectedAdditionalServiceIds ?? new List<Guid>();
                var additionalServices = await context.AdditionalServices
                    .Where(s => selectedServiceIds.Contains(s.Id) && s.VenueId == bookingDto.VenueId)
                    .ToListAsync();
                if (selectedServiceIds.Any(id => !additionalServices.Any(s => s.Id == id)))
                    return MethodResult<BookingDto>.Fail("One or more selected additional services are invalid.");

                // Validate package services for custom packages
                var selectedPackageServiceIds = bookingDto.SelectedPackageServiceIds ?? new List<Guid>();
                var packageServices = new List<PackageService>();
                if (selectedPackage != null && selectedPackageServiceIds.Any())
                {
                    packageServices = await context.PackageServices
                        .Where(ps => selectedPackageServiceIds.Contains(ps.Id) && ps.PackageId == selectedPackage.Id)
                        .ToListAsync();
                    if (selectedPackageServiceIds.Any(id => !packageServices.Any(ps => ps.Id == id)))
                        return MethodResult<BookingDto>.Fail("One or more selected package services are invalid.");
                }

                // Calculate timing
                DateTime endTime;
                int durationHours = bookingDto.ModifiedDurationHours ?? selectedPackage?.BaseDurationHours ?? (int)bookingDto.Duration;
                endTime = bookingDto.StartTime.AddDays(durationHours).AddTicks(-1);

                if (endTime <= bookingDto.StartTime)
                    return MethodResult<BookingDto>.Fail("End time must be after start time.");

                // Check availability
                var startDate = bookingDto.StartTime.Date;
                var endDate = endTime.Date;
                var hasConflict = await context.Bookings
                    .AnyAsync(b => b.VenueId == bookingDto.VenueId &&
                                   b.Status != "Canceled" &&
                                   (b.StartTime.Date <= endDate && b.EndTime.Date >= startDate));
                if (hasConflict)
                    return MethodResult<BookingDto>.Fail("Venue is already booked for the requested day(s).");

                // Calculate total costs
                decimal additionalServicesCost = additionalServices.Sum(s => s.Price);
                decimal packageServicesCost = packageServices.Where(ps => !ps.IsIncludedInPackage).Sum(ps => ps.Price);
                decimal totalCost = basePackageCost + capacityExtensionCost + durationExtensionCost + additionalServicesCost + packageServicesCost;

                // Create booking
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    VenueId = bookingDto.VenueId,
                    PackageId = bookingDto.PackageId,
                    UserId = bookingDto.UserId,
                    StartTime = bookingDto.StartTime,
                    EndTime = endTime,
                    IsCustomPackage = bookingDto.IsCustomPackage,
                    CustomPackageName = bookingDto.CustomPackageName,
                    ModifiedCapacity = bookingDto.ModifiedCapacity,
                    ModifiedDurationHours = bookingDto.ModifiedDurationHours,
                    BasePackageCost = basePackageCost,
                    CapacityExtensionCost = capacityExtensionCost,
                    DurationExtensionCost = durationExtensionCost,
                    AdditionalServicesCost = additionalServicesCost,
                    TotalCost = totalCost,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Pending
                };

                // Add additional services
                foreach (var service in additionalServices)
                {
                    booking.BookingAdditionalServices.Add(new BookingAdditionalService
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        AdditionalServiceId = service.Id,
                        PriceAtBooking = service.Price,
                        Quantity = 1
                    });
                }

                // Add package services for custom packages
                foreach (var packageService in packageServices)
                {
                    booking.BookingPackageServices.Add(new BookingPackageService
                    {
                        Id = Guid.NewGuid(),
                        BookingId = booking.Id,
                        PackageServiceId = packageService.Id,
                        PriceAtBooking = packageService.Price,
                        Quantity = 1,
                        IsIncludedInPackage = packageService.IsIncludedInPackage
                    });
                }

                context.Bookings.Add(booking);
                await context.SaveChangesAsync();

                return MethodResult<BookingDto>.Ok(MapToDto(booking, venue.Name));
            }
            catch (Exception ex)
            {
                return MethodResult<BookingDto>.Fail($"Failed to create booking: {ex.Message}");
            }
        }

        public async Task<MethodResult<BookingWithUserDto>> GetBookingByIdAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var booking = await context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.User)
                    .Include(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                    .Include(b => b.BookingPackageServices)
                        .ThenInclude(bps => bps.PackageService)
                    .Include(b => b.Package)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                    return MethodResult<BookingWithUserDto>.Fail("Booking not found.");

                var result = MapToDtoWithUserInfo(booking);
                return MethodResult<BookingWithUserDto>.Ok(result);
            }
            catch (Exception ex)
            {
                return MethodResult<BookingWithUserDto>.Fail($"Failed to get booking: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<BookingDto>>> GetBookingsByVenueAsync(Guid venueId)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues.FindAsync(venueId);
                if (venue == null)
                    return MethodResult<List<BookingDto>>.Fail("Venue not found.");

                var bookings = await context.Bookings
                    .Where(b => b.VenueId == venueId && b.Status != "Canceled")
                    .Include(b => b.Venue)
                    .Include(b => b.Package)
                    .OrderBy(b => b.StartTime)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => MapToDto(b, venue.Name)).ToList();
                return MethodResult<List<BookingDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<BookingDto>>.Fail($"Failed to get bookings: {ex.Message}");
            }
        }

        public async Task<MethodResult> CancelBookingAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var booking = await context.Bookings.FindAsync(id);
                if (booking == null)
                    return MethodResult.Fail("Booking not found.");
                booking.Status = "Canceled";
                booking.UpdatedAt = DateTime.UtcNow;
                context.Bookings.Update(booking);
                await context.SaveChangesAsync();
                return MethodResult.Ok();
            }
            catch (Exception ex)
            {
                return MethodResult.Fail($"Failed to cancel booking: {ex.Message}");
            }
        }

        public async Task<MethodResult<VenueAvailabilityDto>> CheckVenueAvailabilityAsync(Guid venueId, DateTime startTime, double duration)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues.FindAsync(venueId);
                if (venue == null)
                    return MethodResult<VenueAvailabilityDto>.Fail("Venue not found.");
                if (venue.Status == "Maintenance")
                    return MethodResult<VenueAvailabilityDto>.Ok(new VenueAvailabilityDto { IsAvailable = false });

                DateTime endTime = startTime.AddDays(duration).AddTicks(-1);

                var startDate = startTime.Date;
                var endDate = endTime.Date;
                var hasConflict = await context.Bookings
                    .AnyAsync(b => b.VenueId == venueId &&
                                   b.Status != "Canceled" &&
                                   (b.StartTime.Date <= endDate && b.EndTime.Date >= startDate));

                var bookedDates = await context.Bookings
                    .Where(b => b.VenueId == venueId && b.Status != "Canceled")
                    .Select(b => new { b.StartTime, b.EndTime })
                    .ToListAsync();

                var bookedDateList = new List<DateTime>();
                foreach (var booking in bookedDates)
                {
                    var currentDate = booking.StartTime.Date;
                    while (currentDate <= booking.EndTime.Date)
                    {
                        bookedDateList.Add(currentDate);
                        currentDate = currentDate.AddDays(1);
                    }
                }

                return MethodResult<VenueAvailabilityDto>.Ok(new VenueAvailabilityDto
                {
                    IsAvailable = !hasConflict,
                    BookedDates = bookedDateList.Distinct().OrderBy(d => d).ToList()
                });
            }
            catch (Exception ex)
            {
                return MethodResult<VenueAvailabilityDto>.Fail($"Failed to check availability: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var bookings = await context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.User)
                    .Include(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                    .Include(b => b.BookingPackageServices)
                        .ThenInclude(bps => bps.PackageService)
                    .Include(b => b.Package)
                    .Where(b => b.UserId == userId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => MapToDtoWithUserInfo(b)).ToList();
                return MethodResult<List<BookingWithUserDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail($"Failed to get user bookings: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var bookings = await context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.User)
                    .Include(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                    .Include(b => b.BookingPackageServices)
                        .ThenInclude(bps => bps.PackageService)
                    .Include(b => b.Package)
                    .Where(b => b.UserId == userId && b.StartTime >= startDate && b.EndTime <= endDate)
                    .OrderBy(b => b.StartTime)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => MapToDtoWithUserInfo(b)).ToList();
                return MethodResult<List<BookingWithUserDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail($"Failed to get user bookings: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<BookingWithUserDto>>> GetAllBookingsWithUserInfoAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var bookings = await context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.User)
                    .Include(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                    .Include(b => b.BookingPackageServices)
                        .ThenInclude(bps => bps.PackageService)
                    .Include(b => b.Package)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => MapToDtoWithUserInfo(b)).ToList();
                return MethodResult<List<BookingWithUserDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail($"Failed to get bookings: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsWithUserInfoByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var bookings = await context.Bookings
                    .Include(b => b.Venue)
                    .Include(b => b.User)
                    .Include(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                    .Include(b => b.BookingPackageServices)
                        .ThenInclude(bps => bps.PackageService)
                    .Include(b => b.Package)
                    .Where(b => b.StartTime >= startDate && b.EndTime <= endDate)
                    .OrderBy(b => b.StartTime)
                    .ToListAsync();

                var bookingDtos = bookings.Select(b => MapToDtoWithUserInfo(b)).ToList();
                return MethodResult<List<BookingWithUserDto>>.Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail($"Failed to get bookings: {ex.Message}");
            }
        }

        private BookingWithUserDto MapToDtoWithUserInfo(Booking booking)
        {
            var additionalServices = new List<AdditionalServiceDto>();
            var packageServices = new List<PackageServiceDto>();

            // Safely handle AdditionalServices
            if (booking.BookingAdditionalServices != null)
            {
                foreach (var bas in booking.BookingAdditionalServices)
                {
                    if (bas.AdditionalService != null)
                    {
                        additionalServices.Add(new AdditionalServiceDto
                        {
                            Id = bas.AdditionalService.Id,
                            Name = bas.AdditionalService.Name ?? "Unknown Service",
                            Description = bas.AdditionalService.Description ?? string.Empty,
                            Price = bas.AdditionalService.Price,
                            VenueId = bas.AdditionalService.VenueId,
                            VenueName = booking.Venue?.Name ?? string.Empty,
                            Category = (Shared.Dtos.Venue.ServiceCategory)bas.AdditionalService.Category
                        });
                    }
                }
            }

            // Safely handle PackageServices
            if (booking.BookingPackageServices != null)
            {
                foreach (var bps in booking.BookingPackageServices)
                {
                    if (bps.PackageService != null)
                    {
                        packageServices.Add(new PackageServiceDto
                        {
                            Id = bps.PackageService.Id,
                            Name = bps.PackageService.Name ?? "Unknown Service",
                            Description = bps.PackageService.Description ?? string.Empty,
                            Price = bps.PackageService.Price,
                            IsIncludedInPackage = bps.PackageService.IsIncludedInPackage,
                            IsAvailableForCustomization = bps.PackageService.IsAvailableForCustomization
                        });
                    }
                }
            }

            return new BookingWithUserDto
            {
                Id = booking.Id,
                VenueId = booking.VenueId,
                VenueName = booking.Venue?.Name ?? string.Empty,
                PackageId = booking.PackageId,
                PackageName = booking.Package?.Name ?? string.Empty,
                UserId = booking.UserId,
                UserEmail = booking.User?.Email ?? string.Empty,
                UserFullName = $"{booking.User?.Name ?? "Unknown"}".Trim(),
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                IsCustomPackage = booking.IsCustomPackage,
                CustomPackageName = booking.CustomPackageName,
                ModifiedCapacity = booking.ModifiedCapacity,
                ModifiedDurationHours = booking.ModifiedDurationHours,
                BasePackageCost = booking.BasePackageCost,
                CapacityExtensionCost = booking.CapacityExtensionCost,
                DurationExtensionCost = booking.DurationExtensionCost,
                AdditionalServicesCost = booking.AdditionalServicesCost,
                TotalCost = booking.TotalCost,
                Status = booking.Status ?? "Unknown",
                PaymentStatus = booking.PaymentStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                AdditionalServices = additionalServices,
                PackageServices = packageServices
            };
        }

        private BookingDto MapToDto(Booking booking, string venueName)
        {
            return new BookingDto
            {
                Id = booking.Id,
                VenueId = booking.VenueId,
                VenueName = venueName,
                PackageId = booking.PackageId,
                PackageName = booking.Package?.Name ?? string.Empty,
                UserId = booking.UserId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                IsCustomPackage = booking.IsCustomPackage,
                CustomPackageName = booking.CustomPackageName,
                TotalCost = booking.TotalCost,
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus.ToString(),
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}