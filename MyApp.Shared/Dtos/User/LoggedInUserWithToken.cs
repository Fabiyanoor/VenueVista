namespace MyApp.Shared.Dtos.User;

public record LoggedInUserWithToken(LoggedinUser User, string Jwt);