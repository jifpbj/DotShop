// ============================================================
// InventoryController.cs — Stock Management (Admin only)
// ============================================================
// Both endpoints require Admin role — customers never need to see
// or change raw stock levels.
//
//   GET   /api/inventory/low-stock?threshold=5  — products running low
//   PATCH /api/inventory/{productId}/adjust     — change stock quantity
// ============================================================

using DotShop.API.DTOs.Products;
using DotShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Applies to ALL endpoints in this controller
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // -------------------------------------------------------
    // GET /api/inventory/low-stock
    // GET /api/inventory/low-stock?threshold=10
    // -------------------------------------------------------
    // Useful for a warehouse dashboard — shows items that need restocking.
    // threshold defaults to 5 if not provided in the query string.
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock(
        [FromQuery] int threshold = 5)
    {
        var products = await _inventoryService.GetLowStockAsync(threshold);
        return Ok(products);
    }

    // -------------------------------------------------------
    // PATCH /api/inventory/{productId}/adjust
    // -------------------------------------------------------
    // PATCH is used instead of PUT because we're updating a single field,
    // not replacing the entire resource.
    //
    // Body: a plain integer (the delta)
    //   Positive: +20 = received 20 new units from supplier
    //   Negative:  -3 = 3 units found damaged and removed
    [HttpPatch("{productId}/adjust")]
    public async Task<IActionResult> AdjustStock(string productId, [FromBody] int quantityDelta)
    {
        await _inventoryService.AdjustStockAsync(productId, quantityDelta);
        return NoContent();
    }
}
