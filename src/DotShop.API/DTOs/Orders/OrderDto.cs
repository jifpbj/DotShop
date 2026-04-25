namespace DotShop.API.DTOs.Orders;

// -------------------------------------------------------
// OrderDto — Shape of an order in API responses
// -------------------------------------------------------
// Returned by GET /api/orders and POST /api/orders.
// The nested OrderItemResponseDto carries the snapshotted product
// details (name, price) as they were at time of purchase.
public record OrderDto(
    string Id,
    string CustomerId,
    List<OrderItemResponseDto> Items,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);

// -------------------------------------------------------
// OrderItemResponseDto — One line item in an OrderDto
// -------------------------------------------------------
// Separate from OrderItemDto (the inbound request type) to keep
// inbound and outbound shapes distinct and independently evolvable.
public record OrderItemResponseDto(
    string ProductId,
    string ProductName,
    string ConditionGrade,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal  // UnitPrice * Quantity — pre-calculated for the UI
);
