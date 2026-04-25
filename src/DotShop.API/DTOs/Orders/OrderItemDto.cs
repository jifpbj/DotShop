using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Orders;

// -------------------------------------------------------
// OrderItemDto — One product line inside a CreateOrderRequest
// -------------------------------------------------------
// The client only needs to tell us which product and how many.
// The server looks up the real price from MongoDB — we never
// trust a price sent by the client.
public record OrderItemDto(
    [Required] string ProductId,
    [Range(1, 100)] int Quantity
);
