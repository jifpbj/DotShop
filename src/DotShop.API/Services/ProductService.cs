// ============================================================
// ProductService.cs — Product Business Logic
// ============================================================
// Sits between ProductsController and IProductRepository.
// Responsibilities:
//   - Map between Product model and ProductDto
//   - Apply partial update logic (only changed fields)
//   - Delegate all database access to the repository
// ============================================================

using DotShop.API.DTOs.Products;
using DotShop.API.Services.Interfaces;
using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;

namespace DotShop.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    // The repository is injected by the DI container.
    // ProductService never creates a ProductRepository directly —
    // it only knows about the IProductRepository contract.
    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    // -------------------------------------------------------
    // GetProductsAsync
    // -------------------------------------------------------
    public async Task<IEnumerable<ProductDto>> GetProductsAsync(string? category, string? conditionGrade)
    {
        var products = await _repo.GetAllAsync(category, conditionGrade);

        // LINQ Select projects each Product model into a ProductDto.
        // This is the "mapping" step — similar to what AutoMapper does,
        // but done manually so every field is explicit and easy to understand.
        return products.Select(ToDto);
    }

    // -------------------------------------------------------
    // GetProductByIdAsync
    // -------------------------------------------------------
    public async Task<ProductDto?> GetProductByIdAsync(string id)
    {
        var product = await _repo.GetByIdAsync(id);

        // The null-conditional operator ?. means: if product is null,
        // return null. Otherwise, call ToDto(product).
        return product is null ? null : ToDto(product);
    }

    // -------------------------------------------------------
    // CreateProductAsync
    // -------------------------------------------------------
    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        // Build a new Product model from the request DTO.
        // Id is left as null — MongoDB generates it on InsertOneAsync.
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            ConditionGrade = request.ConditionGrade,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repo.CreateAsync(product);

        // After InsertOneAsync, product.Id is populated by MongoDB.
        return ToDto(product);
    }

    // -------------------------------------------------------
    // UpdateProductAsync — Partial update
    // -------------------------------------------------------
    public async Task<bool> UpdateProductAsync(string id, UpdateProductRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        // Only overwrite fields that were actually provided in the request.
        // The '??' (null-coalescing) operator means: use the request value
        // if it's not null, otherwise keep the existing value.
        existing.Name          = request.Name          ?? existing.Name;
        existing.Description   = request.Description   ?? existing.Description;
        existing.Category      = request.Category      ?? existing.Category;
        existing.ConditionGrade = request.ConditionGrade ?? existing.ConditionGrade;
        existing.Price         = request.Price         ?? existing.Price;
        existing.StockQuantity = request.StockQuantity ?? existing.StockQuantity;
        existing.ImageUrl      = request.ImageUrl      ?? existing.ImageUrl;

        await _repo.UpdateAsync(id, existing);
        return true;
    }

    // -------------------------------------------------------
    // DeleteProductAsync
    // -------------------------------------------------------
    public async Task<bool> DeleteProductAsync(string id)
    {
        return await _repo.DeleteAsync(id);
    }

    // -------------------------------------------------------
    // ToDto — Private mapping helper
    // -------------------------------------------------------
    // Centralises the Product → ProductDto conversion so it's
    // defined in one place. If the DTO shape changes, we update here only.
    private static ProductDto ToDto(Product p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.Category,
        p.ConditionGrade,
        p.Price,
        p.StockQuantity,
        p.ImageUrl
    );
}
