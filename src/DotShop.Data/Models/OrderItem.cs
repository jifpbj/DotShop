// ============================================================
// OrderItem.cs — A Single Line Item Within an Order
// ============================================================
// This class represents ONE product inside a customer's order.
// For example, if a customer buys 2 phones and 1 laptop, the
// order will have two OrderItem objects.
//
// KEY DESIGN DECISION — Embedded Document (not a separate collection):
//   In SQL databases, order items would be a separate table joined to orders.
//   In MongoDB, we EMBED them directly inside the Order document.
//
//   Why? Because when you retrieve an order, you ALWAYS want its items too.
//   Embedding avoids an extra database round-trip and keeps related data together.
//
// KEY DESIGN DECISION — Data Snapshot:
//   We copy the product's name, price, and condition grade at purchase time.
//   This means if the product's price changes next week, the historical order
//   still shows what the customer actually paid. This is called "denormalization"
//   and is intentional here — correctness over storage efficiency.
// ============================================================

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotShop.Data.Models;

public class OrderItem
{
    // A reference back to the product — useful if you want to look up
    // the current live product listing from an order history page.
    // [BsonRepresentation] converts between C# string and MongoDB ObjectId,
    // just like on the top-level model classes.
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductId { get; set; } = null!;

    // Snapshot of the product name at time of purchase.
    // If the seller renames "iPhone 13" to "Apple iPhone 13 (Refurb Grade A)",
    // old orders still show the name as it was when the customer bought it.
    public string ProductName { get; set; } = null!;

    // Snapshot of the condition grade at time of purchase.
    public string ConditionGrade { get; set; } = null!;

    // The ACTUAL price charged — snapshot at purchase time.
    // If a sale ends and the price goes back up, this number doesn't change.
    public decimal UnitPrice { get; set; }

    // How many of this product the customer ordered.
    public int Quantity { get; set; }

    // A computed helper — not stored in MongoDB (no [BsonElement] needed).
    // [BsonIgnore] tells the MongoDB driver to skip this field entirely.
    // It's calculated on the fly whenever you read this object in C#.
    [BsonIgnore]
    public decimal LineTotal => UnitPrice * Quantity;
}
