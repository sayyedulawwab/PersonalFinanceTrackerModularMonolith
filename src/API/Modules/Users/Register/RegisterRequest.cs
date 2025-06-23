namespace API.Modules.Users.Register;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);