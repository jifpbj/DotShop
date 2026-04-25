// ============================================================
// AuthService.cs — Registration, Login, and JWT Generation
// ============================================================
// Handles all authentication logic:
//   - Registering new customers (hashing passwords, creating records)
//   - Logging in (verifying passwords, issuing JWTs)
//   - Generating signed JWT tokens (private helper)
// ============================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotShop.API.DTOs.Auth;
using DotShop.API.Services.Interfaces;
using DotShop.Data.Configuration;
using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DotShop.API.Services;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _repo;
    private readonly JwtSettings _jwtSettings;

    // IOptions<JwtSettings> is how .NET injects configuration objects.
    // .Value unwraps the typed settings from the IOptions wrapper.
    public AuthService(ICustomerRepository repo, IOptions<JwtSettings> jwtOptions)
    {
        _repo = repo;
        _jwtSettings = jwtOptions.Value;
    }

    // -------------------------------------------------------
    // RegisterAsync
    // -------------------------------------------------------
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check for duplicate email before creating the account.
        var existing = await _repo.GetByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("An account with this email already exists.");

        // BCrypt.HashPassword generates a salted hash.
        // The work factor (cost) defaults to 11 — higher = slower to crack, slower to hash.
        // We never store the plain-text password.
        var customer = new Customer
        {
            Email        = request.Email.ToLowerInvariant(), // normalise to lowercase
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName    = request.FirstName,
            LastName     = request.LastName,
            Role         = "Customer",
            CreatedAt    = DateTime.UtcNow
        };

        await _repo.CreateAsync(customer);

        return new AuthResponse(GenerateJwtToken(customer), customer.Id, customer.Email, customer.Role);
    }

    // -------------------------------------------------------
    // LoginAsync
    // -------------------------------------------------------
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var customer = await _repo.GetByEmailAsync(request.Email);

        // BCrypt.Verify compares the plain-text password against the stored hash.
        // Returns false if the customer doesn't exist OR the password is wrong —
        // same result either way, so attackers can't detect which case it is.
        if (customer is null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
            return null;

        return new AuthResponse(GenerateJwtToken(customer), customer.Id, customer.Email, customer.Role);
    }

    // -------------------------------------------------------
    // GenerateJwtToken — Private: build and sign a JWT
    // -------------------------------------------------------
    // A JWT has three parts: Header.Payload.Signature
    //
    // The Payload contains "claims" — key/value facts about the user.
    // We embed CustomerId, Email, and Role so every API endpoint can
    // read them from the token without hitting the database.
    //
    // The Signature is an HMAC-SHA256 hash of Header + Payload, keyed
    // with SecretKey. Any tampering with the payload invalidates the signature.
    private string GenerateJwtToken(Customer customer)
    {
        var claims = new[]
        {
            // NameIdentifier is the standard claim for a user's unique ID.
            // Controllers read this via: User.FindFirstValue(ClaimTypes.NameIdentifier)
            new Claim(ClaimTypes.NameIdentifier, customer.Id),
            new Claim(ClaimTypes.Email,          customer.Email),
            new Claim(ClaimTypes.Role,           customer.Role)
        };

        // SymmetricSecurityKey wraps the secret bytes used to sign the token.
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _jwtSettings.Issuer,
            audience:           _jwtSettings.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        // WriteToken serialises the JwtSecurityToken into the compact
        // "header.payload.signature" string the client stores and sends back.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
