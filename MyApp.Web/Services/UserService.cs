using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities.Bookings;
using System.Collections.Generic;

namespace MyApp.Web.Services;
public class UserService : IUserService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IDbContextFactory<DataContext> contextFactory, IPasswordHasher<User> passwordHasher)
    {
        _contextFactory = contextFactory;
        _passwordHasher = passwordHasher;
    }

    public async Task<MethodResult<List<UserSummaryDto>>> GetAllUsersAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var users = await context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            var dtos = users.Select(u => new UserSummaryDto
            {
                Id = u.Id,
                Name = u.Name ?? string.Empty,
                Email = u.Email ?? string.Empty,
                ContactNumber = u.ContactNumber ?? string.Empty,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            }).ToList();
            return MethodResult<List<UserSummaryDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return MethodResult<List<UserSummaryDto>>.Fail($"Failed to get users: {ex.Message}");
        }
    }

    public async Task<MethodResult<UserDetailsDto>> GetUserDetailsAsync(Guid id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var user = await context.Users
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Venue)
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.BookingAdditionalServices)
                        .ThenInclude(bas => bas.AdditionalService)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return MethodResult<UserDetailsDto>.Fail("User not found.");

            var dto = new UserDetailsDto
            {
                Id = user.Id,
                Name = user.Name ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ContactNumber = user.ContactNumber ?? string.Empty,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Bookings = user.Bookings
                    .Select(b => MapToDtoWithUserInfo(b, user))
                    .OrderByDescending(b => b.CreatedAt)
                    .ToList()
            };
            return MethodResult<UserDetailsDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            return MethodResult<UserDetailsDto>.Fail($"Failed to get user details: {ex.Message}");
        }
    }

    public async Task<MethodResult> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
                return MethodResult.Fail("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                    return MethodResult.Fail("Email already in use.");
                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.ContactNumber))
                user.ContactNumber = dto.ContactNumber;

            // ADD THIS: Update the role
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                user.Role = dto.Role;
            }

            context.Users.Update(user);
            await context.SaveChangesAsync();
            return MethodResult.Ok();
        }
        catch (Exception ex)
        {
            return MethodResult.Fail($"Failed to update user: {ex.Message}");
        }
    }

    private BookingWithUserDto MapToDtoWithUserInfo(Booking booking, User user)
    {
        var additionalServices = new List<AdditionalServiceDto>();
        if (booking.BookingAdditionalServices != null)
        {
            foreach (var bas in booking.BookingAdditionalServices.Where(bas => bas.AdditionalService != null))
            {
                additionalServices.Add(new AdditionalServiceDto
                {
                    Id = bas.AdditionalService.Id,
                    Name = bas.AdditionalService.Name ?? "Unknown Service",
                    Description = bas.AdditionalService.Description ?? string.Empty,
                    Price = bas.AdditionalService.Price,
                    VenueId = bas.AdditionalService.VenueId,
                    VenueName = booking.Venue?.Name ?? string.Empty
                });
            }
        }

        return new BookingWithUserDto
        {
            Id = booking.Id,
            VenueId = booking.VenueId,
            VenueName = booking.Venue?.Name ?? string.Empty,
            UserId = user.Id,
            UserEmail = user.Email ?? string.Empty,
            UserFullName = user.Name ?? "Unknown",
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            TotalCost = booking.TotalCost,
            Status = booking.Status ?? "Unknown",
            PaymentStatus = booking.PaymentStatus.ToString(),
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            AdditionalServices = additionalServices
        };
    }
}