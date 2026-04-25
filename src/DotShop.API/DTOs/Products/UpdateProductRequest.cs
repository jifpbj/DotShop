using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Products;

// -------------------------------------------------------
// UpdateProductRequest — Body of PUT /api/products/{id} (Admin only)
// -------------------------------------------------------
// All fields are nullable (using '?') so the client can send only the
// fields that need to change. The service layer applies only non-null values.
// This is a "partial update" pattern — simpler than PATCH for a demo.
public record UpdateProductRequest(
    string? Name,
    string? Description,
    string? Category,
    [RegularExpression("^[ABC]$", ErrorMessage = "ConditionGrade must be A, B, or C")]
    string? ConditionGrade,
    [Range(0.01, 99999)] decimal? Price,
    [Range(0, int.MaxValue)] int? StockQuantity,
    string? ImageUrl
);
