using CommunityToolkit.Maui.Core;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using MyApp.Services;
using MyApp.Shared.Services;
using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                }).UseMauiCommunityToolkitCore();

            // Add device-specific services used by the MyApp.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IVenueService, VenueService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IAdditionalServiceService, AdditionalServiceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IVenuePackageService, VenuePackageService>();

            ConfigureRefit(builder.Services);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<MauiAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<MauiAuthenticationStateProvider>());

            return builder.Build();
        }

        const string ApiBaseUrl = "https://localhost:7078";

        private static void ConfigureRefit(IServiceCollection services)
        {
            // Create consistent JSON settings for all APIs
            var jsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings)
            };

            // Configure Auth API
            services.AddRefitClient<IAuthApi>(provider => refitSettings)
                .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(ApiBaseUrl));

            // Configure Venue API with proper settings
            services.AddRefitClient<IVenueApi>(provider =>
            {
                var mauiAuthStateProvider = provider.GetRequiredService<MauiAuthenticationStateProvider>();
                var settings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings),
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(mauiAuthStateProvider.Jwt ?? "")
                };
                return settings;
            })
            .ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = new Uri(ApiBaseUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Configure other APIs with the same settings
            services.AddRefitClient<IBookingApi>(provider =>
            {
                var mauiAuthStateProvider = provider.GetRequiredService<MauiAuthenticationStateProvider>();
                var settings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings),
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(mauiAuthStateProvider.Jwt ?? "")
                };
                return settings;
            })
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(ApiBaseUrl));

            services.AddRefitClient<IAdditionalServiceApi>(provider =>
            {
                var mauiAuthStateProvider = provider.GetRequiredService<MauiAuthenticationStateProvider>();
                var settings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings),
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(mauiAuthStateProvider.Jwt ?? "")
                };
                return settings;
            })
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(ApiBaseUrl));

            services.AddRefitClient<IUserApi>(provider =>
            {
                var mauiAuthStateProvider = provider.GetRequiredService<MauiAuthenticationStateProvider>();
                var settings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings),
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(mauiAuthStateProvider.Jwt ?? "")
                };
                return settings;
            })
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(ApiBaseUrl));

            services.AddRefitClient<IVenuePackageApi>(provider =>
            {
                var mauiAuthStateProvider = provider.GetRequiredService<MauiAuthenticationStateProvider>();
                var settings = new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer(jsonSettings),
                    AuthorizationHeaderValueGetter = (_, __) => Task.FromResult(mauiAuthStateProvider.Jwt ?? "")
                };
                return settings;
            })
          .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(ApiBaseUrl));
        }
    }
}