﻿namespace API.Modules.Users.Auth;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);