// ============================================================
// OrderRepository.cs — MongoDB Queries for Orders
// ============================================================
// Implements IOrderRepository using the MongoDB .NET Driver.
// Orders embed their OrderItems directly (no separate collection join needed).
// ============================================================

using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;
using MongoDB.Driver;

namespace DotShop.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(MongoDbContext context)
    {
        _orders = context.Orders;
    }

    // -------------------------------------------------------
    // GetByCustomerIdAsync — All orders for one customer
    // -------------------------------------------------------
    // SortByDescending on CreatedAt shows the most recent orders first —
    // the natural expectation for an order history page.
    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.CustomerId, customerId);

        return await _orders
            .Find(filter)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    // -------------------------------------------------------
    // GetByIdAsync — Fetch a single order by its ID
    // -------------------------------------------------------
    public async Task<Order?> GetByIdAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        return await _orders.Find(filter).FirstOrDefaultAsync();
    }

    // -------------------------------------------------------
    // CreateAsync — Persist a new order
    // -------------------------------------------------------
    // Called after OrderService has validated stock and built the Order object.
    // MongoDB writes the generated _id back into order.Id.
    public async Task CreateAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    // -------------------------------------------------------
    // UpdateStatusAsync — Advance an order through its lifecycle
    // -------------------------------------------------------
    // Uses $set to update only the Status field — the rest of the
    // order document (items, total, customer) remains unchanged.
    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var update = Builders<Order>.Update.Set(o => o.Status, status);

        var result = await _orders.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}
