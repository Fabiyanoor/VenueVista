using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Services;
using System;
using System.Threading.Tasks;

namespace MyApp.Web.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto bookingDto)
        {
            var result = await _bookingService.CreateBookingAsync(bookingDto);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookingsWithUserInfo()
        {
            var result = await _bookingService.GetAllBookingsWithUserInfoAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetBookingsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate >= endDate)
                return BadRequest(new { error = "Start date must be before end date." });

            var result = await _bookingService.GetBookingsWithUserInfoByDateRangeAsync(startDate, endDate);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("venue/{venueId:guid}")]
        public async Task<IActionResult> GetBookingsByVenue(Guid venueId)
        {
            var result = await _bookingService.GetBookingsByVenueAsync(venueId);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetBookingsByUserId(Guid userId)
        {
            var result = await _bookingService.GetBookingsByUserIdAsync(userId);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("user/{userId:guid}/date-range")]
        public async Task<IActionResult> GetBookingsByUserIdAndDateRange(Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate >= endDate)
                return BadRequest(new { error = "Start date must be before end date." });
            var result = await _bookingService.GetBookingsByUserIdAndDateRangeAsync(userId, startDate, endDate);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("venue/{venueId:guid}/availability")]
        public async Task<IActionResult> CheckVenueAvailability(Guid venueId, [FromQuery] DateTime startTime, [FromQuery] double duration)
        {
            var result = await _bookingService.CheckVenueAvailabilityAsync(venueId, startTime, duration);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        // NEW: Get bookings by package
        [HttpGet("package/{packageId:guid}")]
        public async Task<IActionResult> GetBookingsByPackage(Guid packageId)
        {
            // This would require adding a new method to IBookingService
            // You can implement this if needed
            return BadRequest(new { error = "Endpoint not implemented yet" });
        }
    }
}