namespace DotShop.API.DTOs.Customers;

// -------------------------------------------------------
// CustomerDto — Safe public view of a customer's profile
// -------------------------------------------------------
// Returned by GET /api/customers/me.
// Deliberately excludes PasswordHash and Role — sensitive fields
// that the client UI never needs to display.
public record CustomerDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);
