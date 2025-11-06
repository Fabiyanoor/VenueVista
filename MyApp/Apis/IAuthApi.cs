using Microsoft.AspNetCore.Http;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using Refit;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<MethodResult> RegisterAsync(RegisterModel model);

    [Post("/api/auth/login")]
    Task<MethodResult<LoggedInUserWithToken>> LoginAsync(LoginModel model); 
}
