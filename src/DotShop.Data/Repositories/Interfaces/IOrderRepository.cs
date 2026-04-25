// ============================================================
// IOrderRepository.cs — Order Repository Contract
// ============================================================
// Defines the database operations available for Order documents.
// See IProductRepository.cs for a full explanation of why interfaces
// are used here instead of concrete classes.
// ============================================================

using DotShop.Data.Models;

namespace DotShop.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    // Retrieve all orders belonging to a specific customer.
    // Used on the "My Orders" / order history page in the Angular app.
    Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId);

    // Retrieve a single order by its MongoDB ObjectId string.
    // Returns null (Order?) if no order with that ID exists.
    Task<Order?> GetByIdAsync(string id);

    // Insert a new completed order into the "orders" collection.
    // Called by OrderService after validating stock and building the Order object.
    Task CreateAsync(Order order);

    // Update just the Status field of an existing order.
    // Example progression: "Pending" → "Confirmed" → "Shipped" → "Delivered"
    // Returns true if the order was found and updated, false if the ID was not found.
    Task<bool> UpdateStatusAsync(string id, string status);
}
