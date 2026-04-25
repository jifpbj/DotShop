using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Auth;

// -------------------------------------------------------
// RegisterRequest — Body of POST /api/auth/register
// -------------------------------------------------------
public record RegisterRequest(
    [Required][EmailAddress] string Email,

    // MinLength ensures a password isn't trivially short.
    // A real app would also enforce complexity rules, but this is
    // sufficient for a demo.
    [Required][MinLength(8)] string Password,

    [Required] string FirstName,
    [Required] string LastName
);
