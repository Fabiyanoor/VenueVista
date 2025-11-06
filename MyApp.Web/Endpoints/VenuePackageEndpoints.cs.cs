using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using MyApp.Shared.Services;
using System.Security.Claims;

namespace MyApp.Web.Endpoints;

public static class VenuePackageEndpoints
{
    public static IEndpointRouteBuilder MapVenuePackageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app/packages")
            .RequireAuthorization(p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        // Create package
        group.MapPost("", async (CreateVenuePackageDto packageDto, IVenuePackageService packageService) =>
        {
            var result = await packageService.CreatePackageAsync(packageDto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenuePackageDto>>();

        // Update package
        group.MapPut("{id:guid}", async (Guid id, CreateVenuePackageDto packageDto, IVenuePackageService packageService) =>
        {
            var result = await packageService.UpdatePackageAsync(id, packageDto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenuePackageDto>>();

        // Delete package (soft delete)
        group.MapDelete("{id:guid}", async (Guid id, IVenuePackageService packageService) =>
        {
            var result = await packageService.DeletePackageAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult>();

        // Get package by ID
        group.MapGet("{id:guid}", async (Guid id, IVenuePackageService packageService) =>
        {
            var result = await packageService.GetPackageByIdAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult<VenuePackageDto>>();

        // Get packages by venue ID
        group.MapGet("venue/{venueId:guid}", async (Guid venueId, IVenuePackageService packageService) =>
        {
            var result = await packageService.GetPackagesByVenueIdAsync(venueId);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<VenuePackageDto>>>();

        // Get all packages
        group.MapGet("", async (IVenuePackageService packageService) =>
        {
            var result = await packageService.GetAllPackagesAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<VenuePackageDto>>>();

        // Get filtered packages
        group.MapPost("filter", async (PackageFilterDto filter, IVenuePackageService packageService) =>
        {
            var result = await packageService.GetFilteredPackagesAsync(filter);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<VenuePackageDto>>>();

        // Get filter options (for UI dropdowns, ranges, etc.)
        group.MapGet("filter-options", async (IVenuePackageService packageService) =>
        {
            var result = await packageService.GetFilterOptionsAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<PackageFilterOptionsDto>>();

        return app;
    }
}