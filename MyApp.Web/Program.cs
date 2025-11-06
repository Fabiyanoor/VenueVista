using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.Shared.Services;
using MyApp.Web;
using MyApp.Web.Components;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities;
using MyApp.Web.Endpoints;
using MyApp.Web.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register cross-project services
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// FIXED: Use only DbContextFactory with proper configuration
builder.Services.AddDbContextFactory<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'Default' not found.");
    }
    options.UseSqlServer(connectionString);
});

// Register Identity's password hasher
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

// Register AuthService for login/register functionality
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(WebConstants.WebAuthScheme)
    .AddJwtBearer(options =>
    {
        var secureKey = builder.Configuration.GetValue<string>("Jwt:SecureKey");
        if (string.IsNullOrEmpty(secureKey))
        {
            throw new InvalidOperationException("JWT SecureKey not found in configuration.");
        }
        byte[] key = Encoding.UTF8.GetBytes(secureKey);
        var securityKey = new SymmetricSecurityKey(key);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateLifetime = true,
        };
    })
    .AddCookie(WebConstants.WebAuthScheme, options =>
    {
        options.Cookie.Name = WebConstants.WebAuthScheme;
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

// Your VenueService registration (should work as-is)
builder.Services.AddScoped<IVenueService>(provider =>
{
    var contextFactory = provider.GetRequiredService<IDbContextFactory<DataContext>>();
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var logger = provider.GetRequiredService<ILogger<VenueService>>();
    return new VenueService(contextFactory, environment, httpContextAccessor, logger);
});

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
builder.Services.AddScoped<IVenuePackageService, VenuePackageService>();

// Register custom AuthenticationStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, WebAuthStateProvider>();
builder.Services.AddControllers();

// ========== ADD THIS CRITICAL CONFIGURATION FOR MINIMAL APIs ==========
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
});

// Also configure for MVC controllers (if you have any)
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp API", Version = "v1" });
});

var app = builder.Build();

// Add static files configuration BEFORE any mapping or middleware
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "..", "MyApp.Shared", "wwwroot")),
    RequestPath = ""
});

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApp API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MyApp.Shared._Imports).Assembly);

app.MapAuthEnpoints()
   .MapVenueEndpoints()
   .MapBookingEndpoints()
   .MapUserEndpoints()
   .MapVenuePackageEndpoints() 
   .MapAdditionalServiceEndpoints();

app.Run();