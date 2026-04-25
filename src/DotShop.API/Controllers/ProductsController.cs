// ============================================================
// ProductsController.cs — Product Browsing and Management
// ============================================================
// Public endpoints (no auth required):
//   GET /api/products              — list all active products (with optional filters)
//   GET /api/products/{id}         — get one product by ID
//
// Admin-only endpoints:
//   POST   /api/products           — create a new listing
//   PUT    /api/products/{id}      — update an existing listing
//   DELETE /api/products/{id}      — soft-delete a listing
// ============================================================

using DotShop.API.DTOs.Products;
using DotShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // -------------------------------------------------------
    // GET /api/products
    // GET /api/products?category=Laptops
    // GET /api/products?conditionGrade=A
    // GET /api/products?category=Phones&conditionGrade=B
    // -------------------------------------------------------
    // [FromQuery] binds URL query string parameters to method arguments.
    // Both are optional (nullable) — omitting them returns all products.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] string? conditionGrade)
    {
        var products = await _productService.GetProductsAsync(category, conditionGrade);
        return Ok(products);
    }

    // -------------------------------------------------------
    // GET /api/products/{id}
    // -------------------------------------------------------
    // {id} in the route template is a route parameter — it captures
    // the value from the URL path and passes it to the method.
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        // If the service returns null, the product wasn't found.
        // 404 NotFound is the correct HTTP response for a missing resource.
        if (product is null) return NotFound();

        return Ok(product);
    }

    // -------------------------------------------------------
    // POST /api/products  (Admin only)
    // -------------------------------------------------------
    // [Authorize(Roles = "Admin")] checks the "role" claim in the JWT.
    // If the token belongs to a "Customer", this returns 403 Forbidden.
    // If there's no token at all, this returns 401 Unauthorized.
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
    {
        var created = await _productService.CreateProductAsync(request);

        // 201 Created with a Location header pointing to the new resource.
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // -------------------------------------------------------
    // PUT /api/products/{id}  (Admin only)
    // -------------------------------------------------------
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, UpdateProductRequest request)
    {
        var success = await _productService.UpdateProductAsync(id, request);
        if (!success) return NotFound();

        // 204 No Content is the standard response for a successful update
        // that doesn't need to return the updated resource.
        return NoContent();
    }

    // -------------------------------------------------------
    // DELETE /api/products/{id}  (Admin only)
    // -------------------------------------------------------
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _productService.DeleteProductAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
