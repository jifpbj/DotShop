// ============================================================
// CustomersController.cs — Customer Profile Management
// ============================================================
// Authenticated customers can view and update their own profile.
//
//   GET /api/customers/me  — fetch the logged-in customer's profile
//   PUT /api/customers/me  — update first/last name
// ============================================================

using System.Security.Claims;
using DotShop.API.DTOs.Customers;
using DotShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // -------------------------------------------------------
    // GET /api/customers/me
    // -------------------------------------------------------
    // "me" is a well-known REST convention for "the currently authenticated user".
    // This avoids needing to know your own ID upfront — the token is enough.
    [HttpGet("me")]
    public async Task<ActionResult<CustomerDto>> GetMe()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var customer = await _customerService.GetByIdAsync(customerId);
        return Ok(customer);
    }

    // -------------------------------------------------------
    // PUT /api/customers/me
    // -------------------------------------------------------
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateCustomerRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _customerService.UpdateAsync(customerId, request);
        if (!success) return NotFound();
        return NoContent();
    }
}
