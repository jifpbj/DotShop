// ============================================================
// OrdersController.cs — Order Placement and History
// ============================================================
// All endpoints require a valid JWT ([Authorize] at class level).
//
//   GET   /api/orders              — the logged-in customer's order history
//   GET   /api/orders/{id}         — a specific order
//   POST  /api/orders              — place a new order (checkout)
//   PATCH /api/orders/{id}/status  — update order status (Admin only)
// ============================================================

using System.Security.Claims;
using DotShop.API.DTOs.Orders;
using DotShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require a valid JWT
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // -------------------------------------------------------
    // GET /api/orders
    // -------------------------------------------------------
    // Returns the authenticated customer's own orders only.
    // We read the customer's ID from the JWT — they cannot
    // pass a different customer's ID to see others' orders.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
    {
        // User.FindFirstValue reads a specific claim from the JWT token.
        // ClaimTypes.NameIdentifier maps to the "sub" claim we set in AuthService.
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    // -------------------------------------------------------
    // GET /api/orders/{id}
    // -------------------------------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }

    // -------------------------------------------------------
    // POST /api/orders
    // -------------------------------------------------------
    // The Angular cart sends the list of items here when the customer
    // clicks "Place Order". The service validates stock, decrements
    // quantities, and persists the order.
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await _orderService.CreateOrderAsync(customerId, request);

        // 201 Created — return the new order and a Location header pointing to it.
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // -------------------------------------------------------
    // PATCH /api/orders/{id}/status  (Admin only)
    // -------------------------------------------------------
    // Body: a plain string, e.g. "Confirmed", "Shipped", "Delivered"
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] string status)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, status);
        if (!success) return NotFound();
        return NoContent();
    }
}
