using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using System.Security.Claims;

namespace MyApp.Web.Endpoints;

public static class AdditionalServiceEndpoints
{
    public static IEndpointRouteBuilder MapAdditionalServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app/additional-services")
            .RequireAuthorization(p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        // Create additional service
        group.MapPost("", async (CreateAdditionalServiceDto dto, IAdditionalServiceService serviceService, HttpContext httpContext) =>
        {
            

            var result = await serviceService.CreateAsync(dto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<AdditionalServiceDto>>();

        // Update additional service
        group.MapPut("{id:guid}", async (Guid id, CreateAdditionalServiceDto dto, IAdditionalServiceService serviceService, HttpContext httpContext) =>
        {
           
            var result = await serviceService.UpdateAsync(id, dto);
            return Results.Ok(result);
        })
        .Produces<MethodResult<AdditionalServiceDto>>();

        // Delete additional service
        group.MapDelete("{id:guid}", async (Guid id, IAdditionalServiceService serviceService, HttpContext httpContext) =>
        {
            

            var result = await serviceService.DeleteAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult>();

        // Get additional service by ID
        group.MapGet("{id:guid}", async (Guid id, IAdditionalServiceService serviceService) =>
        {
            var result = await serviceService.GetByIdAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult<AdditionalServiceDto>>();

        // Get additional services by venue ID
        group.MapGet("venue/{venueId:guid}", async (Guid venueId, IAdditionalServiceService serviceService) =>
        {
            var result = await serviceService.GetByVenueIdAsync(venueId);
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<AdditionalServiceDto>>>();

        // Get all additional services (admin only)
        group.MapGet("all", async (IAdditionalServiceService serviceService, HttpContext httpContext) =>
        {
          
            var result = await serviceService.GetAllAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<AdditionalServiceDto>>>();

        return app;
    }
}