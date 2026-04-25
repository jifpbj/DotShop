// ============================================================
// InventoryService.cs — Inventory / Stock Management Logic
// ============================================================
// Handles admin-facing stock operations: viewing low-stock alerts
// and manually adjusting quantities (restocking or corrections).
// Order-placement stock decrements go through OrderService instead.
// ============================================================

using DotShop.API.DTOs.Products;
using DotShop.API.Services.Interfaces;
using DotShop.Data.Repositories.Interfaces;

namespace DotShop.API.Services;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _repo;

    public InventoryService(IProductRepository repo)
    {
        _repo = repo;
    }

    // -------------------------------------------------------
    // GetLowStockAsync
    // -------------------------------------------------------
    // Fetches all active products, then filters in memory for those
    // at or below the threshold. For a large catalogue a database-level
    // query (Builders filter with $lte) would be more efficient, but
    // this is clear and correct for a demo.
    public async Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 5)
    {
        var all = await _repo.GetAllAsync();

        return all
            .Where(p => p.StockQuantity <= threshold)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description,
                p.Category, p.ConditionGrade, p.Price, p.StockQuantity, p.ImageUrl));
    }

    // -------------------------------------------------------
    // AdjustStockAsync
    // -------------------------------------------------------
    // Positive delta = new stock received (e.g. +20 units from warehouse)
    // Negative delta = manual correction (e.g. -3 units found damaged)
    public async Task AdjustStockAsync(string productId, int quantityDelta)
    {
        // Verify the product exists before adjusting to give a clear error.
        var product = await _repo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product '{productId}' not found.");

        await _repo.UpdateStockAsync(productId, quantityDelta);
    }
}
