// ============================================================
// JwtSettings.cs — Strongly-Typed JWT Configuration
// ============================================================
// Mirrors the "JwtSettings" section in appsettings.json.
// Used by AuthService to generate tokens and by Program.cs
// to configure the JWT validation middleware.
//
// WHAT IS A JWT (JSON Web Token)?
//   A JWT is a compact, self-contained token that proves identity.
//   When a customer logs in, we generate a JWT and send it back.
//   The customer includes it in every subsequent request (in the
//   Authorization header), and our API trusts it without needing
//   to look up the database on each call.
//
//   A JWT has three parts separated by dots:
//     header.payload.signature
//
//   The PAYLOAD contains "claims" — facts about the user:
//     { "sub": "abc123", "email": "user@example.com", "role": "Customer" }
//
//   The SIGNATURE is a cryptographic hash of the header + payload,
//   signed with SecretKey. Anyone can READ the payload, but only
//   the server (which knows SecretKey) can PRODUCE a valid signature.
//   This means the client cannot tamper with the claims.
// ============================================================

namespace DotShop.Data.Configuration;

public class JwtSettings
{
    // The secret key used to sign and verify tokens.
    // Must be at least 32 characters long for HMAC-SHA256 security.
    // NEVER commit a real secret to git — use environment variables or
    // a secrets manager in production.
    public string SecretKey { get; set; } = null!;

    // The "issuer" claim identifies who created the token.
    // We set this to "DotShop.API" and validate it on every request
    // to ensure the token came from our server and not somewhere else.
    public string Issuer { get; set; } = null!;

    // The "audience" claim identifies who the token is intended for.
    // We set this to "DotShop.Client" — only our Angular app should use these tokens.
    public string Audience { get; set; } = null!;

    // How many minutes the token is valid before it expires.
    // After expiry, the user must log in again to get a fresh token.
    // 60 minutes is a reasonable balance between security and convenience.
    public int ExpiryMinutes { get; set; }
}
