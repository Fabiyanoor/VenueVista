using System.Security.Claims;

namespace MyApp.Shared.Dtos.User;

public record LoggedinUser(Guid Id, string Name, string Role)
{
    public bool IsAdmin => Role == "Admin"; // Derived for compatibility

    public Claim[] ToClaim() =>
        [
            new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.Role, Role) // Add role claim
        ];
}
