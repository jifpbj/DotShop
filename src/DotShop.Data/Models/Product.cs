// ============================================================
// Product.cs — The "Product" Data Model
// ============================================================
// This class represents a single refurbished electronics listing
// in our MongoDB database. Every document in the "products"
// collection maps to an instance of this class.
//
// Think of this like a blueprint for a row in a spreadsheet:
// each Product object holds all the fields for one item for sale.
//
// The attributes (things in [square brackets]) are called "annotations".
// They tell the MongoDB driver how to read and write each field.
// ============================================================

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotShop.Data.Models;

public class Product
{
    // [BsonId] tells MongoDB this field is the unique document identifier (_id).
    // Every MongoDB document must have one — it's like a primary key in SQL.
    //
    // [BsonRepresentation(BsonType.ObjectId)] means:
    //   - Store it in MongoDB as a native ObjectId (a 12-byte binary value MongoDB generates)
    //   - But expose it in C# as a plain string so we don't litter ObjectId types everywhere
    //
    // "null!" is C#'s way of saying: "Trust me, this will never actually be null at runtime —
    // MongoDB will always populate it." The '!' suppresses the nullable compiler warning.
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    // The product's display name, e.g. "Apple MacBook Pro 2021"
    public string Name { get; set; } = null!;

    // A longer human-readable description of the item's condition and specs.
    public string Description { get; set; } = null!;

    // Product category for filtering, e.g. "Laptops", "Phones", "Tablets", "Accessories"
    public string Category { get; set; } = null!;

    // Magnakom's condition grading system — critical for the refurbished electronics domain:
    //   "A" = Like new. No visible cosmetic damage. Full functionality.
    //   "B" = Good. Minor cosmetic wear (small scratches). Full functionality.
    //   "C" = Fair. Visible cosmetic damage. Fully functional.
    // This grade directly affects the price and buyer expectations.
    public string ConditionGrade { get; set; } = null!;

    // Selling price in the store's currency (CAD for Magnakom).
    // 'decimal' is used for money — never use 'float' or 'double' for currency
    // because floating-point arithmetic has rounding errors (e.g. 0.1 + 0.2 ≠ 0.3).
    public decimal Price { get; set; }

    // How many units are currently available to purchase.
    // When an order is placed, this number decreases. If it hits 0, the item is out of stock.
    public int StockQuantity { get; set; }

    // URL to the product's image, stored externally (e.g. a CDN or local static folder).
    // Storing binary image data directly in MongoDB is bad practice — always use a URL.
    public string ImageUrl { get; set; } = string.Empty;

    // The UTC timestamp when this product was first listed.
    // UTC (Coordinated Universal Time) is always used in databases so there's no
    // ambiguity about time zones. Convert to local time only when displaying to users.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Soft-delete flag. Instead of physically removing a product from the database
    // (which would break historical order references), we set IsActive = false to
    // "hide" it. Queries always filter on IsActive == true.
    public bool IsActive { get; set; } = true;
}
