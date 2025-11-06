using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services;

public class AuthService : IAuthService
{
    private readonly IAuthApi _authApi;
    private readonly MauiAuthenticationStateProvider _mauiAuthenticationStateProvider;

    public AuthService(IAuthApi authApi , MauiAuthenticationStateProvider mauiAuthenticationStateProvider)
    {
        _authApi = authApi;
        
        _mauiAuthenticationStateProvider = mauiAuthenticationStateProvider;
    }

    private string? _jwt = null;
    public async Task<MethodResult<LoggedinUser>> LoginAsync(LoginModel model)
    {
        try
        {
          

            var result = await _authApi.LoginAsync(model);

            if (!result.IsSuccess)
            {
                return MethodResult<LoggedinUser>.Fail(result.Error ?? "Login failed");
            }

            if (result.Data?.User == null)
            {
                return MethodResult<LoggedinUser>.Fail("No user data returned from login");
            }
            _jwt = result.Data.Jwt;
            return MethodResult<LoggedinUser>.Ok(result.Data.User);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<LoggedinUser>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<LoggedinUser>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> RegisterAsync(RegisterModel model)
    {
        try
        {
            if (model == null)
            {
                return MethodResult.Fail("Invalid registration data provided");
            }

            var result = await _authApi.RegisterAsync(model);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Registration failed");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            return MethodResult.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public  Task<MethodResult> PlatformLoginAsync(LoggedinUser user)
    {
        var userWithJwt = new LoggedInUserWithToken(user, _jwt);
       _= _mauiAuthenticationStateProvider.Login(userWithJwt);
        return Task.FromResult(MethodResult.Ok());
    }

    public  Task<MethodResult> PlatformLogoutAsync()
    { 
        _mauiAuthenticationStateProvider.logout();
        return Task.FromResult(MethodResult.Ok());
    }
}