using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using System.Security.Claims;

namespace MyApp.Web.Endpoints;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app/bookings")      

            .RequireAuthorization(p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        // Create booking
        group.MapPost("", async (CreateBookingDto bookingDto, IBookingService bookingService, HttpContext httpContext) =>
        {
            var result = await bookingService.CreateBookingAsync(bookingDto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<BookingDto>>();

        // Get booking by ID
        group.MapGet("{id:guid}", async (Guid id, IBookingService bookingService) =>
        {
            var result = await bookingService.GetBookingByIdAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult<BookingWithUserDto>>();

        // Get bookings by venue
        group.MapGet("venue/{venueId:guid}", async (Guid venueId, IBookingService bookingService) =>
        {
            var result = await bookingService.GetBookingsByVenueAsync(venueId);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<BookingDto>>>();

        // Cancel booking
        group.MapPut("{id:guid}/cancel", async (Guid id, IBookingService bookingService) =>
        {
            var result = await bookingService.CancelBookingAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult>();

        // Check venue availability
        group.MapGet("availability/{venueId:guid}", async (Guid venueId, [FromQuery] DateTime startTime, [FromQuery] double duration, IBookingService bookingService) =>
        {
            var result = await bookingService.CheckVenueAvailabilityAsync(venueId, startTime, duration);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenueAvailabilityDto>>();

        // Get all bookings with user info (admin only)
        group.MapGet("all", async (IBookingService bookingService, HttpContext httpContext) =>
        {
           

            var result = await bookingService.GetAllBookingsWithUserInfoAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<BookingWithUserDto>>>();

        // Get bookings by date range with user info (admin only)
        group.MapGet("date-range", async ([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, IBookingService bookingService, HttpContext httpContext) =>
        {
          

            var result = await bookingService.GetBookingsWithUserInfoByDateRangeAsync(startDate, endDate);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<BookingWithUserDto>>>();

        // Get bookings by user ID
        group.MapGet("user/{userId:guid}", async (Guid userId, IBookingService bookingService, HttpContext httpContext) =>
        {
            var user = httpContext.User;
            var authenticatedUserId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            if (userId != authenticatedUserId)
            {
                return Results.Forbid();
            }
            var result = await bookingService.GetBookingsByUserIdAsync(userId);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<BookingWithUserDto>>>();

        // Get bookings by user ID and date range
        group.MapGet("user/{userId:guid}/date-range", async (Guid userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, IBookingService bookingService, HttpContext httpContext) =>
        {
            var user = httpContext.User;
            var authenticatedUserId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            if (userId != authenticatedUserId)
            {
                return Results.Forbid();
            }
            if (startDate >= endDate)
            {
                return Results.BadRequest(new { error = "Start date must be before end date." });
            }
            var result = await bookingService.GetBookingsByUserIdAndDateRangeAsync(userId, startDate, endDate);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<BookingWithUserDto>>>();

        return app;
    }

}