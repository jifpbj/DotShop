using DotShop.API.DTOs.Products;

namespace DotShop.API.Services.Interfaces;

public interface IInventoryService
{
    // Returns products whose StockQuantity is at or below the threshold.
    // Defaults to 5 — useful for a "low stock" dashboard alert.
    Task<IEnumerable<ProductDto>> GetLowStockAsync(int threshold = 5);

    // Adjusts stock by a delta (positive = restock, negative = sold).
    // Throws KeyNotFoundException if the product does not exist.
    Task AdjustStockAsync(string productId, int quantityDelta);
}
