// ============================================================
// DTOs — What Are They and Why Do We Need Them?
// ============================================================
// DTO = Data Transfer Object.
// A DTO is a simple container for data that travels across a boundary —
// in our case, between the HTTP request/response and the service layer.
//
// WHY NOT USE THE MODEL CLASSES (Product, Customer, etc.) DIRECTLY?
//
//   1. Security: The Customer model contains PasswordHash. If we returned
//      Customer objects directly from the API, we'd leak password hashes
//      to the client. DTOs let us expose only what's safe.
//
//   2. Stability: If we rename a MongoDB field, the API response shape
//      stays the same (we just update the DTO mapping). Client code
//      (Angular) doesn't break.
//
//   3. Validation: Request DTOs carry [Required], [Range], [RegularExpression]
//      attributes that validate incoming data at the controller boundary,
//      before it ever reaches business logic.
//
// RULE: Models ↔ Database.  DTOs ↔ HTTP boundary.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Auth;

// -------------------------------------------------------
// LoginRequest — Body of POST /api/auth/login
// -------------------------------------------------------
// 'record' is a C# type that is immutable and value-based.
// It's perfect for DTOs because request data shouldn't change
// after it's been received and validated.
public record LoginRequest(
    // [Required] means the field must be present and non-empty.
    // If missing, the framework returns 400 Bad Request automatically.
    [Required][EmailAddress] string Email,
    [Required] string Password
);
