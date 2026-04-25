using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Products;

// -------------------------------------------------------
// CreateProductRequest — Body of POST /api/products (Admin only)
// -------------------------------------------------------
public record CreateProductRequest(
    [Required] string Name,
    [Required] string Description,
    [Required] string Category,

    // RegularExpression validates the value is exactly "A", "B", or "C".
    // The pattern ^[ABC]$ means: start (^), one character that is A, B, or C,
    // then end ($). Anything else returns 400 Bad Request.
    [Required][RegularExpression("^[ABC]$", ErrorMessage = "ConditionGrade must be A, B, or C")]
    string ConditionGrade,

    // Range ensures price is between 1 cent and $99,999 — prevents
    // accidental free listings or absurdly large values.
    [Range(0.01, 99999)] decimal Price,

    [Range(0, int.MaxValue)] int StockQuantity,

    string ImageUrl
);
