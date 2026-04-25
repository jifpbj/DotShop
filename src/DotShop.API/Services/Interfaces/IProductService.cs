// ============================================================
// IProductService.cs — Product Service Contract
// ============================================================
// The service layer sits between controllers and repositories.
//
//   Controller  →  Service  →  Repository  →  MongoDB
//
// Controllers handle HTTP (routing, status codes, request parsing).
// Services handle business logic (validation, mapping, rules).
// Repositories handle database queries (Find, Insert, Update).
//
// This separation means:
//   - Business logic is testable without HTTP or a real database
//   - Controllers stay thin and readable
//   - Swapping MongoDB for another DB only changes the repository,
//     not the service or controller
// ============================================================

using DotShop.API.DTOs.Products;

namespace DotShop.API.Services.Interfaces;

public interface IProductService
{
    // Returns all active products, optionally filtered.
    // Maps Product model → ProductDto (safe public shape).
    Task<IEnumerable<ProductDto>> GetProductsAsync(string? category, string? conditionGrade);

    // Returns one product, or null if not found.
    Task<ProductDto?> GetProductByIdAsync(string id);

    // Creates a new listing. Returns the saved product as a DTO.
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);

    // Applies non-null fields from the request to the existing product.
    // Returns false if the product ID was not found.
    Task<bool> UpdateProductAsync(string id, UpdateProductRequest request);

    // Soft-deletes the product. Returns false if not found.
    Task<bool> DeleteProductAsync(string id);
}
