// ============================================================
// Order.cs — The "Order" Data Model
// ============================================================
// Represents a completed purchase placed by a customer.
// Stored in the MongoDB "orders" collection.
//
// An order is essentially a container that links:
//   - WHO bought it (CustomerId)
//   - WHAT they bought (Items — a list of embedded OrderItem objects)
//   - HOW MUCH they paid (TotalAmount)
//   - WHERE it is in the fulfilment pipeline (Status)
// ============================================================

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotShop.Data.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    // The MongoDB ObjectId of the customer who placed this order.
    // This is a "soft reference" — MongoDB has no foreign keys like SQL,
    // so we store the ID as a string and join manually in the service layer if needed.
    [BsonRepresentation(BsonType.ObjectId)]
    public string CustomerId { get; set; } = null!;

    // The list of products in this order, stored as embedded documents.
    // See OrderItem.cs for why they're embedded and why prices are snapshots.
    // Initialised to an empty list so we never get a NullReferenceException.
    public List<OrderItem> Items { get; set; } = new();

    // The total cost of the entire order (sum of all line item totals).
    // We store this rather than recalculate it each time for performance
    // and to preserve the exact amount charged (even if prices later change).
    public decimal TotalAmount { get; set; }

    // The current fulfilment status — moves through a lifecycle:
    //   Pending   → Order received, not yet confirmed by warehouse
    //   Confirmed → Warehouse has acknowledged and is preparing the shipment
    //   Shipped   → Package is with the courier
    //   Delivered → Customer has received the package
    //   Cancelled → Order was cancelled before shipment
    //
    // Admins update this status via PATCH /api/orders/{id}/status
    public string Status { get; set; } = "Pending";

    // When the order was placed — UTC timestamp for audit trail.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
