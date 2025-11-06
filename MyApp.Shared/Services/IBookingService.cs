using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;

namespace MyApp.Shared.Services
{
    public interface IBookingService
    {
        Task<MethodResult<List<BookingWithUserDto>>> GetAllBookingsWithUserInfoAsync();
        Task<MethodResult<List<BookingWithUserDto>>> GetBookingsWithUserInfoByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<MethodResult<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto);
        Task<MethodResult<BookingWithUserDto>> GetBookingByIdAsync(Guid id);
            Task<MethodResult<List<BookingDto>>> GetBookingsByVenueAsync(Guid venueId);
        Task<MethodResult> CancelBookingAsync(Guid id);
        Task<MethodResult<VenueAvailabilityDto>> CheckVenueAvailabilityAsync(Guid venueId, DateTime startTime, double duration);
         Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAsync(Guid userId);
         Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    }
}