namespace DotShop.API.DTOs.Products;

// -------------------------------------------------------
// ProductDto — Shape of a product in API responses
// -------------------------------------------------------
// Returned by GET /api/products and GET /api/products/{id}.
// Does NOT include IsActive (an internal field) or CreatedAt
// (not needed by the storefront UI).
public record ProductDto(
    string Id,
    string Name,
    string Description,
    string Category,
    string ConditionGrade,  // "A", "B", or "C"
    decimal Price,
    int StockQuantity,
    string ImageUrl
);
