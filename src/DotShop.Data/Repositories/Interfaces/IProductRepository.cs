// ============================================================
// IProductRepository.cs — Product Repository Contract (Interface)
// ============================================================
// An "interface" is a contract — it defines WHAT operations exist
// without specifying HOW they work. The actual implementation lives
// in ProductRepository.cs.
//
// WHY USE AN INTERFACE?
//   1. Testability: Unit tests can swap in a fake/mock repository
//      that returns hardcoded data, so tests never touch the real database.
//   2. Swappability: If we later switch from MongoDB to PostgreSQL,
//      we write a new SqlProductRepository and register it in Program.cs —
//      nothing else in the app changes.
//   3. Dependency Inversion: Services depend on this interface, not the
//      concrete class. This is the "D" in SOLID design principles.
//
// The "Task<T>" return type means every method is ASYNCHRONOUS.
// Async/await lets the server handle many requests at once — while one
// request waits for MongoDB to respond, the thread is free to serve others.
// ============================================================

using DotShop.Data.Models;

namespace DotShop.Data.Repositories.Interfaces;

public interface IProductRepository
{
    // Retrieve all active products, with optional filters.
    // null means "no filter" — pass a value to narrow results.
    // Example: GetAllAsync("Laptops", "A") → only Grade-A laptops
    Task<IEnumerable<Product>> GetAllAsync(string? category = null, string? conditionGrade = null);

    // Retrieve a single product by its MongoDB ObjectId string.
    // Returns null (Product?) if no product with that ID exists.
    Task<Product?> GetByIdAsync(string id);

    // Insert a new product document into the "products" collection.
    Task CreateAsync(Product product);

    // Replace an existing product document with updated data.
    Task UpdateAsync(string id, Product product);

    // Adjust the StockQuantity of a product by a delta value.
    // Positive delta = restock (+10 units added)
    // Negative delta = sold  (-1 when an order is placed)
    // This is safer than a full UpdateAsync because it avoids
    // race conditions on concurrent purchases (uses $inc operator).
    Task UpdateStockAsync(string id, int quantityDelta);

    // Soft-delete: sets IsActive = false instead of physically removing the document.
    // Returns true if the product was found and updated, false if not found.
    Task<bool> DeleteAsync(string id);
}
