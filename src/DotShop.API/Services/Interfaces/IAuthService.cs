using DotShop.API.DTOs.Auth;

namespace DotShop.API.Services.Interfaces;

public interface IAuthService
{
    // Verifies credentials and returns a JWT on success.
    // Returns null if the email doesn't exist or the password is wrong.
    // (Returning null rather than throwing keeps timing consistent —
    // we don't want to leak whether the email exists via error type.)
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    // Hashes the password, creates the customer record, and returns a JWT.
    // Throws InvalidOperationException if the email is already registered.
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}
