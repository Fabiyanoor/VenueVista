using Microsoft.IdentityModel.Tokens;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.Web.Endpoints;

public static class AuthEndpoint
{
    public static IEndpointRouteBuilder MapAuthEnpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").AllowAnonymous();

        group.MapPost("register", async (RegisterModel model, IAuthService authService) =>
        {
            return Results.Ok(await authService.RegisterAsync(model));
        })
        .Produces<MethodResult>();


        group.MapPost("Login", async (LoginModel model, IAuthService authService, IConfiguration configuration) =>


        {
            var result = await authService.LoginAsync(model);
            MethodResult<LoggedInUserWithToken> apiResult;
            if (result.IsSuccess)
            {
                var jwt = GenerateJwt(result.Data, configuration);
                var loggedInUserWithToken = new LoggedInUserWithToken(result.Data, jwt);
                apiResult = MethodResult<LoggedInUserWithToken>.Ok(loggedInUserWithToken);
            }
            else
            {
                apiResult = new MethodResult<LoggedInUserWithToken>(result.IsSuccess, null, result.Error);
            }
            return Results.Ok(apiResult);
        })
        .Produces<MethodResult<LoggedInUserWithToken>>();


        return app;
    }

    private static string GenerateJwt(LoggedinUser user, IConfiguration configuration)
    {
        var secureKey = configuration.GetValue<string>("Jwt:SecureKey");
        byte[] key = Encoding.UTF8.GetBytes(secureKey);
        var securityKey = new SymmetricSecurityKey(key);
        var signingCreds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>(user.ToClaim()) // Use existing claims from ToClaim()
    {
        new Claim("IsAdmin", user.IsAdmin.ToString()) // Add IsAdmin claim
    };
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("Jwt:Issuer"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: signingCreds);
        var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return jwt;
    }
}


