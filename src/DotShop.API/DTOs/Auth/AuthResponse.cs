namespace DotShop.API.DTOs.Auth;

// -------------------------------------------------------
// AuthResponse — Returned by both /login and /register
// -------------------------------------------------------
// The Angular app stores this token in localStorage and attaches
// it to every subsequent request via the HTTP interceptor.
//
// We also return CustomerId, Email, and Role so the Angular app
// can display the user's name and adjust the UI (e.g. show Admin
// controls) without making an extra API call.
public record AuthResponse(
    string Token,       // The signed JWT string, e.g. "eyJhbGci..."
    string CustomerId,  // MongoDB ObjectId — used to fetch "my orders"
    string Email,
    string Role         // "Customer" or "Admin"
);
