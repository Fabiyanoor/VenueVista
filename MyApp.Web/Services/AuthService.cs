using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities;
using System.Security.Claims;

namespace MyApp.Web.Services;

// Service that handles authentication-related logic
public class AuthService : IAuthService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDbContextFactory<DataContext> contextFactory,
        IPasswordHasher<User> passwordHasher,
        IHttpContextAccessor httpContextAccessor)
    {
        _passwordHasher = passwordHasher;
        _contextFactory = contextFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    // Login method: checks user credentials
    public async Task<MethodResult<LoggedinUser>> LoginAsync(LoginModel model)
    {
        var context = _contextFactory.CreateDbContext();

        // 1. Check if the user exists in DB
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
            return MethodResult<LoggedinUser>.Fail("Account does not exist.");

        // 2. Verify hashed password with stored hash
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, model.Password);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return MethodResult<LoggedinUser>.Fail("Invalid credentials.");

        // 3. Return basic user info
        // Return basic user info
        // 3. Return basic user info
        // 3. Return basic user info
        var loggedInUser = new LoggedinUser(user.Id, user.Name ?? string.Empty, user.Role);
        return MethodResult<LoggedinUser>.Ok(loggedInUser);
    }


    public async Task<MethodResult> PlatformLoginAsync(LoggedinUser user)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return MethodResult.Fail("No HttpContext available.");

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
        new Claim("IsAdmin", user.IsAdmin.ToString()) 
    };

        var identity = new ClaimsIdentity(claims, WebConstants.WebAuthScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(WebConstants.WebAuthScheme, principal);

        return MethodResult.Ok();
    }
    public async Task<MethodResult> PlatformLogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return MethodResult.Fail("No HttpContext available.");

        await httpContext.SignOutAsync(WebConstants.WebAuthScheme);
        return MethodResult.Ok();
    }

    // Registration method: creates a new user
    public async Task<MethodResult> RegisterAsync(RegisterModel model)
    {
        var context = _contextFactory.CreateDbContext();

        // Ensure email is unique
        var isEmailExist = await context.Users.AnyAsync(u => u.Email == model.Email);
        if (isEmailExist)
            return MethodResult.Fail("Email is already registered.");

        // Create user entity
        // Create user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = model.Name ?? string.Empty,
            Email = model.Email ?? string.Empty,
            ContactNumber = model.ContactNumber ?? "",
            Role = "User", // Default to "User"
            CreatedAt = DateTime.UtcNow
        };

        // Hash the password before saving
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password ?? string.Empty);



        // Save to DB
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return MethodResult.Ok();
    }
}
