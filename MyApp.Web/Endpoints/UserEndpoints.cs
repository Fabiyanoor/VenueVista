using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;

namespace MyApp.Web.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/app/users")
            .RequireAuthorization(p =>
            {
                p.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });

        // Get all users
        group.MapGet("", async (IUserService userService) =>
        {
            var result = await userService.GetAllUsersAsync();
            return Results.Ok(result);
        })
        .Produces<MethodResult<List<UserSummaryDto>>>();

        // Get user details by ID
        group.MapGet("{id:guid}", async (Guid id, IUserService userService) =>
        {
            var result = await userService.GetUserDetailsAsync(id);
            return Results.Ok(result);
        })
        .Produces<MethodResult<UserDetailsDto>>();

        // Update user by ID
        group.MapPut("{id:guid}", async (Guid id, UpdateUserDto dto, IUserService userService) =>
        {
            if (dto == null)
                return Results.BadRequest(new { error = "Invalid user data provided." });
            var result = await userService.UpdateUserAsync(id, dto);
            return Results.Ok(result);
        })
        .Produces<MethodResult>();

        return app;
    }
}