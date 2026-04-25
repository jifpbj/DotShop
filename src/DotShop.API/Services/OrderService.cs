// ============================================================
// OrderService.cs — Order Placement and Retrieval Logic
// ============================================================
// The most business-logic-heavy service. Placing an order involves:
//   1. Looking up each product to get the real (server-side) price
//   2. Checking that enough stock is available
//   3. Decrementing stock for each item
//   4. Building and persisting the Order document
//
// We never trust the price sent by the Angular client — we always
// fetch the price from MongoDB ourselves.
// ============================================================

using DotShop.API.DTOs.Orders;
using DotShop.API.Services.Interfaces;
using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;

namespace DotShop.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;

    // Both repositories are injected — OrderService needs to read products
    // and write orders in a single operation.
    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    // -------------------------------------------------------
    // GetOrdersByCustomerAsync
    // -------------------------------------------------------
    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId)
    {
        var orders = await _orderRepo.GetByCustomerIdAsync(customerId);
        return orders.Select(ToDto);
    }

    // -------------------------------------------------------
    // GetOrderByIdAsync
    // -------------------------------------------------------
    public async Task<OrderDto?> GetOrderByIdAsync(string id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        return order is null ? null : ToDto(order);
    }

    // -------------------------------------------------------
    // CreateOrderAsync — The core checkout logic
    // -------------------------------------------------------
    public async Task<OrderDto> CreateOrderAsync(string customerId, CreateOrderRequest request)
    {
        var orderItems = new List<OrderItem>();
        decimal total = 0;

        // Process each item the customer wants to buy.
        foreach (var itemDto in request.Items)
        {
            // Step 1: Look up the real product from the database.
            var product = await _productRepo.GetByIdAsync(itemDto.ProductId)
                ?? throw new KeyNotFoundException($"Product '{itemDto.ProductId}' not found.");

            // Step 2: Verify there's enough stock to fulfil the requested quantity.
            if (product.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. " +
                    $"Requested: {itemDto.Quantity}, Available: {product.StockQuantity}");

            // Step 3: Snapshot the product details into the order line item.
            // Price is taken from the DB (not the client) — prevents price tampering.
            orderItems.Add(new OrderItem
            {
                ProductId     = product.Id,
                ProductName   = product.Name,
                ConditionGrade = product.ConditionGrade,
                UnitPrice     = product.Price,
                Quantity      = itemDto.Quantity
            });

            // Step 4: Atomically decrement stock in MongoDB.
            await _productRepo.UpdateStockAsync(product.Id, -itemDto.Quantity);

            total += product.Price * itemDto.Quantity;
        }

        // Step 5: Persist the completed order.
        var order = new Order
        {
            CustomerId  = customerId,
            Items       = orderItems,
            TotalAmount = total,
            Status      = "Pending",
            CreatedAt   = DateTime.UtcNow
        };

        await _orderRepo.CreateAsync(order);

        return ToDto(order);
    }

    // -------------------------------------------------------
    // UpdateOrderStatusAsync
    // -------------------------------------------------------
    public async Task<bool> UpdateOrderStatusAsync(string id, string status)
    {
        return await _orderRepo.UpdateStatusAsync(id, status);
    }

    // -------------------------------------------------------
    // ToDto — Order model → OrderDto
    // -------------------------------------------------------
    private static OrderDto ToDto(Order o) => new(
        o.Id,
        o.CustomerId,
        o.Items.Select(i => new OrderItemResponseDto(
            i.ProductId,
            i.ProductName,
            i.ConditionGrade,
            i.UnitPrice,
            i.Quantity,
            i.LineTotal
        )).ToList(),
        o.TotalAmount,
        o.Status,
        o.CreatedAt
    );
}
