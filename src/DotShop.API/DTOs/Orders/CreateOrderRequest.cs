using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Orders;

// -------------------------------------------------------
// CreateOrderRequest — Body of POST /api/orders
// -------------------------------------------------------
// The Angular cart sends this when the customer clicks "Place Order".
// Items is the list of products and quantities in the cart.
public record CreateOrderRequest(
    [Required][MinLength(1, ErrorMessage = "Order must contain at least one item")]
    List<OrderItemDto> Items
);
