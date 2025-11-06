using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services;

public class BookingService : IBookingService
{
    private readonly IBookingApi _bookingApi;

    public BookingService(IBookingApi bookingApi)
    {
        _bookingApi = bookingApi;
    }

    public async Task<MethodResult<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto)
    {
        try
        {
            if (bookingDto == null)
            {
                return MethodResult<BookingDto>.Fail("Invalid booking data provided");
            }

            var result = await _bookingApi.CreateBookingAsync(bookingDto);

            if (!result.IsSuccess)
            {
                return MethodResult<BookingDto>.Fail(result.Error ?? "Failed to create booking");
            }

            if (result.Data == null)
            {
                return MethodResult<BookingDto>.Fail("No booking data returned");
            }

            return MethodResult<BookingDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<BookingDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<BookingDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<BookingWithUserDto>> GetBookingByIdAsync(Guid id)
    {
        try
        {
            Console.WriteLine($"DEBUG: Calling GetBookingByIdAsync with ID: {id}");

            if (id == Guid.Empty)
            {
                return MethodResult<BookingWithUserDto>.Fail("Invalid booking ID provided");
            }

            var result = await _bookingApi.GetBookingByIdAsync(id);

            Console.WriteLine($"DEBUG: API Response - Success: {result.IsSuccess}, Error: {result.Error}");

            if (!result.IsSuccess)
            {
                return MethodResult<BookingWithUserDto>.Fail(result.Error ?? "Failed to retrieve booking");
            }

            if (result.Data == null)
            {
                return MethodResult<BookingWithUserDto>.Fail("No booking data returned");
            }

            return MethodResult<BookingWithUserDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"DEBUG: HTTP Error: {ex.Message}");
            return MethodResult<BookingWithUserDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: General Error: {ex.Message}");
            return MethodResult<BookingWithUserDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<BookingDto>>> GetBookingsByVenueAsync(Guid venueId)
    {
        try
        {
            if (venueId == Guid.Empty)
            {
                return MethodResult<List<BookingDto>>.Fail("Invalid venue ID provided");
            }

            var result = await _bookingApi.GetBookingsByVenueAsync(venueId);

            if (!result.IsSuccess)
            {
                return MethodResult<List<BookingDto>>.Fail(result.Error ?? "Failed to retrieve bookings");
            }

            if (result.Data == null)
            {
                return MethodResult<List<BookingDto>>.Fail("No booking data returned");
            }

            return MethodResult<List<BookingDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<BookingDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<BookingDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> CancelBookingAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult.Fail("Invalid booking ID provided");
            }

            var result = await _bookingApi.CancelBookingAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Failed to cancel booking");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            return MethodResult.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<VenueAvailabilityDto>> CheckVenueAvailabilityAsync(Guid venueId, DateTime startTime, double duration)
    {
        try
        {
            if (venueId == Guid.Empty)
            {
                return MethodResult<VenueAvailabilityDto>.Fail("Invalid venue ID provided");
            }

            var result = await _bookingApi.CheckVenueAvailabilityAsync(venueId, startTime, duration);

            if (!result.IsSuccess)
            {
                return MethodResult<VenueAvailabilityDto>.Fail(result.Error ?? "Failed to check availability");
            }

            if (result.Data == null)
            {
                return MethodResult<VenueAvailabilityDto>.Fail("No availability data returned");
            }

            return MethodResult<VenueAvailabilityDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenueAvailabilityDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenueAvailabilityDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<BookingWithUserDto>>> GetAllBookingsWithUserInfoAsync()
    {
        try
        {
            var result = await _bookingApi.GetAllBookingsWithUserInfoAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail(result.Error ?? "Failed to retrieve bookings");
            }

            if (result.Data == null)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("No booking data returned");
            }

            return MethodResult<List<BookingWithUserDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsWithUserInfoByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var result = await _bookingApi.GetBookingsWithUserInfoByDateRangeAsync(startDate, endDate);

            if (!result.IsSuccess)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail(result.Error ?? "Failed to retrieve bookings");
            }

            if (result.Data == null)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("No booking data returned");
            }

            return MethodResult<List<BookingWithUserDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }


    public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAsync(Guid userId)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("Invalid user ID provided");
            }
            var result = await _bookingApi.GetBookingsByUserIdAsync(userId);
            if (!result.IsSuccess)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail(result.Error ?? "Failed to retrieve bookings");
            }
            if (result.Data == null)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("No booking data returned");
            }
            return MethodResult<List<BookingWithUserDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<BookingWithUserDto>>> GetBookingsByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("Invalid user ID provided");
            }
            if (startDate >= endDate)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("Start date must be before end date");
            }
            var result = await _bookingApi.GetBookingsByUserIdAndDateRangeAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail(result.Error ?? "Failed to retrieve bookings");
            }
            if (result.Data == null)
            {
                return MethodResult<List<BookingWithUserDto>>.Fail("No booking data returned");
            }
            return MethodResult<List<BookingWithUserDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<BookingWithUserDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}