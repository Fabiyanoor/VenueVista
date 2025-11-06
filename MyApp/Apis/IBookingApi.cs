using Microsoft.AspNetCore.Http;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using Refit;

[Headers("Authorization: Bearer")]
public interface IBookingApi
{
    [Post("/api/app/bookings")]
    Task<MethodResult<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto);

    [Get("/api/app/bookings/{id}")]
    Task<MethodResult<BookingWithUserDto>> GetBookingByIdAsync(Guid id);

    [Get("/api/app/bookings/venue/{venueId}")]
    Task<MethodResult<List<BookingDto>>> GetBookingsByVenueAsync(Guid venueId);

    [Put("/api/app/bookings/{id}/cancel")]
    Task<MethodResult> CancelBookingAsync(Guid id);

    [Get("/api/app/bookings/availability/{venueId}")]
    Task<MethodResult<VenueAvailabilityDto>> CheckVenueAvailabilityAsync(Guid venueId, DateTime startTime, double duration);

    [Get("/api/app/bookings/all")]
    Task<MethodResult<List<BookingWithUserDto>>> GetAllBookingsWithUserInfoAsync();

    [Get("/api/app/bookings/date-range")]
    Task<MethodResult<List<BookingWithUserDto>>> GetBookingsWithUserInfoByDateRangeAsync(DateTime startDate, DateTime endDate);

    [Get("/api/app/bookings/user/{userId}")]
    Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAsync(Guid userId);

    [Get("/api/app/bookings/user/{userId}/date-range")]
    Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
}