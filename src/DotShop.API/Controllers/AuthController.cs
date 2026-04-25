// ============================================================
// AuthController.cs — Registration and Login Endpoints
// ============================================================
// Handles the two public endpoints that don't require authentication:
//   POST /api/auth/register  — create a new customer account
//   POST /api/auth/login     — verify credentials, get a JWT
//
// These are the only endpoints without [Authorize] — everyone needs to
// be able to register and log in before they can do anything else.
// ============================================================

using DotShop.API.DTOs.Auth;
using DotShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotShop.API.Controllers;

// [ApiController] enables:
//   - Automatic model validation (returns 400 if [Required] fields are missing)
//   - Automatic binding from request body (no need for [FromBody] on every param)
//   - ProblemDetails error responses for validation failures
//
// [Route("api/[controller]")] sets the base URL to /api/auth
// [controller] is a placeholder — replaced with the class name minus "Controller".
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // The DI container injects the registered IAuthService implementation.
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // -------------------------------------------------------
    // POST /api/auth/register
    // -------------------------------------------------------
    // Creates a new customer account and returns a JWT so the
    // user is immediately logged in without a separate login call.
    //
    // ActionResult<T> means this method can return either:
    //   - 201 Created with an AuthResponse body (success)
    //   - 400 Bad Request (validation failure or duplicate email)
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        // 201 Created is the correct status for a successful resource creation.
        // CreatedAtAction sets the Location header to the new resource's URL.
        return CreatedAtAction(nameof(Register), response);
    }

    // -------------------------------------------------------
    // POST /api/auth/login
    // -------------------------------------------------------
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        // LoginAsync returns null for both "email not found" and "wrong password".
        // We return 401 for both — we don't tell the client which one,
        // so attackers can't enumerate valid email addresses.
        if (response is null)
            return Unauthorized(new { error = "Invalid email or password." });

        return Ok(response);
    }
}
