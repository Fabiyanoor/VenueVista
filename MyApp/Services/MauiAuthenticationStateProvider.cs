using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.Services;

public class MauiAuthenticationStateProvider :AuthenticationStateProvider
{


    private static readonly Task<AuthenticationState> _emptyAuthStateTask =  Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public LoggedinUser User { get; private set; }

    public string Jwt {  get; private set; }

    private const string UserStateKey ="user";
  public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {

    if (Preferences.Default.ContainsKey(UserStateKey))
        {
            //Get From Preference
            var SerlizeUserState = Preferences.Default.Get<string?>(UserStateKey, null);
            if(!string.IsNullOrWhiteSpace(SerlizeUserState)  )
            {
               var userWithJwt =JsonSerializer.Deserialize<LoggedInUserWithToken>(SerlizeUserState);
              var authStateTask=  Login(userWithJwt!);
                return authStateTask;
               }
                 }

    return _emptyAuthStateTask;
    }



    private static  Task<AuthenticationState> GetEmptyState()
    {

        
        var authStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        return authStateTask;
    }
    public Task<AuthenticationState> Login(LoggedInUserWithToken userWithJwt)
    {
        (User, Jwt) = userWithJwt;

        Preferences.Default.Set<string>(UserStateKey, JsonSerializer.Serialize(userWithJwt));

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userWithJwt.User.Id.ToString()),
        new Claim(ClaimTypes.Name, userWithJwt.User.Name ?? string.Empty),
        new Claim(ClaimTypes.Role, userWithJwt.User.Role), // Add Role claim
        new Claim("IsAdmin", userWithJwt.User.Role == "Admin" ? "True" : "False") // Add IsAdmin claim
    };

        var identity = new ClaimsIdentity(claims, "Venue");
        var principal = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(principal);
        var authStateTask = Task.FromResult(authState);
        NotifyAuthenticationStateChanged(authStateTask);
        return authStateTask;
    }

    public void logout()
    {

      
        NotifyAuthenticationStateChanged(_emptyAuthStateTask);
        (User, Jwt) = (null ,null);

        Preferences.Default.Remove(UserStateKey);
    }
}
