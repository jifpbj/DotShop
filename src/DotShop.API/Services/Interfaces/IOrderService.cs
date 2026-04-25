using DotShop.API.DTOs.Orders;

namespace DotShop.API.Services.Interfaces;

public interface IOrderService
{
    // All orders belonging to the authenticated customer.
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId);

    // Single order by ID. Returns null if not found.
    Task<OrderDto?> GetOrderByIdAsync(string id);

    // Validates stock, decrements quantities, and persists the order.
    // Throws InvalidOperationException if any item is out of stock.
    Task<OrderDto> CreateOrderAsync(string customerId, CreateOrderRequest request);

    // Advances the order status (Admin only).
    // Returns false if the order ID was not found.
    Task<bool> UpdateOrderStatusAsync(string id, string status);
}
