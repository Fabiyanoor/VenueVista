using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using System.Security.Claims;

namespace MyApp.Web.Endpoints;

public static class VenueEndpoints
{
    public static IEndpointRouteBuilder MapVenueEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app/venue")
            .RequireAuthorization(p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        // Get all venues
        group.MapGet("", async (IVenueService venueService) =>
        {
            var result = await venueService.GetAllVenuesAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<VenueDto>>>();

        // Get venue by ID
        group.MapGet("{id:guid}", async (Guid id, IVenueService venueService) =>
        {
            var result = await venueService.GetVenueByIdAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenueDto>>();

        // Create a new venue
        group.MapPost("", async (CreateVenueDto venueDto, IVenueService venueService, HttpContext httpContext) =>
        {
            var result = await venueService.CreateVenueAsync(venueDto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenueDto>>();

        // Update an existing venue
        group.MapPut("{id:guid}", async (Guid id, CreateVenueDto venueDto, IVenueService venueService) =>
        {
            var result = await venueService.UpdateVenueAsync(id, venueDto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenueDto>>();

        // Delete a venue
        group.MapDelete("{id:guid}", async (Guid id, IVenueService venueService) =>
        {
            var result = await venueService.DeleteVenueAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult>();

        // Upload venue images




        return app;
    }
}