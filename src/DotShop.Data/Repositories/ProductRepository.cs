// ============================================================
// ProductRepository.cs — MongoDB Queries for Products
// ============================================================
// This class is the IMPLEMENTATION of IProductRepository.
// It contains the actual MongoDB query logic — the "how" behind
// the interface's "what".
//
// KEY CONCEPTS USED HERE:
//
//   Builders<T>.Filter  → builds a MongoDB query filter (like a WHERE clause in SQL)
//   Builders<T>.Update  → builds an update operation (like SET in SQL)
//   Find()              → queries documents matching a filter
//   InsertOneAsync()    → inserts a single document
//   ReplaceOneAsync()   → replaces an entire document with new data
//   UpdateOneAsync()    → updates specific fields of a document
//
//   All methods are async (return Task<T>) so the thread isn't blocked
//   while waiting for MongoDB to respond over the network.
// ============================================================

using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;
using MongoDB.Driver;

namespace DotShop.Data.Repositories;

public class ProductRepository : IProductRepository
{
    // The collection reference from MongoDbContext, injected via the constructor.
    private readonly IMongoCollection<Product> _products;

    // Constructor injection: MongoDbContext is provided by the DI container.
    // We only need the Products collection, so we extract it here once.
    public ProductRepository(MongoDbContext context)
    {
        _products = context.Products;
    }

    // -------------------------------------------------------
    // GetAllAsync — List products with optional filters
    // -------------------------------------------------------
    // Builds a composable filter starting with IsActive == true,
    // then optionally adds Category and/or ConditionGrade conditions.
    //
    // WHY BUILD FILTERS THIS WAY (not raw strings)?
    //   The MongoDB driver's typed Builders API generates safe BSON queries.
    //   Raw string interpolation like $"{{ category: '{category}' }}" would
    //   be vulnerable to NoSQL injection attacks.
    public async Task<IEnumerable<Product>> GetAllAsync(
        string? category = null,
        string? conditionGrade = null
    )
    {
        // Start with: only return products that are not soft-deleted.
        var filter = Builders<Product>.Filter.Eq(p => p.IsActive, true);

        // If a category filter was provided, AND it onto the existing filter.
        // The &= operator combines two filters with a logical AND.
        if (category is not null)
            filter &= Builders<Product>.Filter.Eq(p => p.Category, category);

        if (conditionGrade is not null)
            filter &= Builders<Product>.Filter.Eq(p => p.ConditionGrade, conditionGrade);

        // Find() returns a cursor — ToListAsync() materialises it into a List<Product>.
        return await _products.Find(filter).ToListAsync();
    }

    // -------------------------------------------------------
    // GetByIdAsync — Fetch one product by its ObjectId string
    // -------------------------------------------------------
    public async Task<Product?> GetByIdAsync(string id)
    {
        // ObjectId.Parse converts the string "abc123..." into a MongoDB ObjectId
        // so we can match it against the _id field.
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        // FirstOrDefaultAsync returns the first match, or null if nothing found.
        return await _products.Find(filter).FirstOrDefaultAsync();
    }

    // -------------------------------------------------------
    // CreateAsync — Insert a new product document
    // -------------------------------------------------------
    public async Task CreateAsync(Product product)
    {
        // MongoDB auto-generates the _id (ObjectId) and writes it back
        // into product.Id after the insert completes.
        await _products.InsertOneAsync(product);
    }

    // -------------------------------------------------------
    // UpdateAsync — Replace an existing product document
    // -------------------------------------------------------
    public async Task UpdateAsync(string id, Product product)
    {
        // ReplaceOneAsync replaces the ENTIRE document matched by the filter.
        // The existing _id is preserved — MongoDB won't change it.
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        await _products.ReplaceOneAsync(filter, product);
    }

    // -------------------------------------------------------
    // UpdateStockAsync — Atomically adjust stock quantity
    // -------------------------------------------------------
    // Uses MongoDB's $inc (increment) operator to change StockQuantity
    // by the given delta, rather than reading and writing the whole document.
    //
    // WHY ATOMIC? If two customers buy the last unit at the same time,
    // a read-then-write approach could oversell. $inc is atomic at the
    // document level — MongoDB processes it as a single indivisible operation.
    public async Task UpdateStockAsync(string id, int quantityDelta)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        // Inc(field, amount) translates to: { $inc: { stockQuantity: delta } }
        var update = Builders<Product>.Update.Inc(p => p.StockQuantity, quantityDelta);

        await _products.UpdateOneAsync(filter, update);
    }

    // -------------------------------------------------------
    // DeleteAsync — Soft-delete a product
    // -------------------------------------------------------
    // Sets IsActive = false instead of physically removing the document.
    // This preserves the product reference in historical order records.
    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var update = Builders<Product>.Update.Set(p => p.IsActive, false);

        var result = await _products.UpdateOneAsync(filter, update);

        // ModifiedCount tells us how many documents were actually changed.
        // If 0, the product ID didn't exist.
        return result.ModifiedCount > 0;
    }
}
