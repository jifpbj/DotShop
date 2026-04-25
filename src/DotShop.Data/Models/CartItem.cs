// ============================================================
// CartItem.cs — A Single Item in a Shopping Cart
// ============================================================
// Represents one product that a customer has added to their cart
// but hasn't yet purchased.
//
// KEY DESIGN DECISION — Cart is NOT stored in MongoDB:
//   Unlike products and orders, cart data is transient (temporary).
//   The customer might add items, close the browser, and never buy.
//   We don't want the database filled with millions of abandoned carts.
//
//   Instead, the cart lives in the Angular frontend's memory (via a
//   BehaviorSubject in CartService). When the customer clicks "Place Order",
//   the frontend sends the cart contents to POST /api/orders, which then
//   creates a real, persisted Order document.
//
//   This class is used on the API side purely as a convenience type —
//   it matches the shape of data the Angular cart sends to the order endpoint.
// ============================================================

namespace DotShop.Data.Models;

public class CartItem
{
    // The MongoDB ObjectId of the product (as a string) — used to look up
    // the real product record and verify price/stock when the order is placed.
    public string ProductId { get; set; } = null!;

    // Displayed in the cart UI — copied from the product at the time it was added.
    public string ProductName { get; set; } = null!;

    // The price shown to the customer in the cart.
    // IMPORTANT: The server re-validates this against the live product price
    // in OrderService when the order is placed. We never trust the client's price.
    public decimal Price { get; set; }

    // How many of this product the customer wants to buy.
    public int Quantity { get; set; }

    // Displayed alongside the product name so buyers know what condition they're getting.
    public string ConditionGrade { get; set; } = null!;
}
