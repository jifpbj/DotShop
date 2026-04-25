// ============================================================
// SeedData.cs — Development Database Seeder
// ============================================================
// Populates MongoDB with realistic sample data on first run so the
// app is immediately usable without manual data entry.
//
// Called from Program.cs only when ASPNETCORE_ENVIRONMENT=Development.
// Checks if the products collection is already non-empty before inserting,
// so re-running the app doesn't duplicate the seed records.
// ============================================================

using DotShop.Data;
using DotShop.Data.Models;
using MongoDB.Driver;

namespace DotShop.API;

public static class SeedData
{
    // IServiceProvider gives access to the DI container so we can
    // resolve scoped services (like MongoDbContext) outside of a request.
    public static async Task SeedAsync(IServiceProvider services)
    {
        // CreateScope creates a temporary DI scope — required to resolve
        // scoped services (those registered with AddScoped) outside of a request.
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        // Only seed if the collection is empty — prevents duplicate data on restart.
        // FilterDefinition<T>.Empty matches every document — equivalent to SELECT COUNT(*).
        var count = await context.Products.CountDocumentsAsync(FilterDefinition<Product>.Empty);
        if (count > 0) return;

        // -------------------------------------------------------
        // Sample refurbished electronics — realistic Magnakom inventory
        // -------------------------------------------------------
        var products = new List<Product>
        {
            new() {
                Name           = "Apple MacBook Pro 13\" (2021)",
                Description    = "M1 chip, 8GB RAM, 256GB SSD. Minor scuffs on lid. Battery health 91%.",
                Category       = "Laptops",
                ConditionGrade = "B",
                Price          = 849.99m,
                StockQuantity  = 5,
                ImageUrl       = "/images/macbook-pro-13.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Dell XPS 15 (2022)",
                Description    = "Intel i7-12700H, 16GB RAM, 512GB NVMe. Like new, original box included.",
                Category       = "Laptops",
                ConditionGrade = "A",
                Price          = 1099.00m,
                StockQuantity  = 3,
                ImageUrl       = "/images/dell-xps-15.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Lenovo ThinkPad X1 Carbon (2020)",
                Description    = "Intel i5-10310U, 8GB RAM, 256GB SSD. Cracked palm rest repaired.",
                Category       = "Laptops",
                ConditionGrade = "C",
                Price          = 449.00m,
                StockQuantity  = 8,
                ImageUrl       = "/images/thinkpad-x1.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Apple iPhone 14 Pro 128GB",
                Description    = "Space Black. Screen replaced with OEM part. Face ID fully functional.",
                Category       = "Phones",
                ConditionGrade = "B",
                Price          = 629.00m,
                StockQuantity  = 12,
                ImageUrl       = "/images/iphone-14-pro.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Samsung Galaxy S23 Ultra 256GB",
                Description    = "Phantom Black. No scratches. All accessories included. Unlocked.",
                Category       = "Phones",
                ConditionGrade = "A",
                Price          = 799.00m,
                StockQuantity  = 7,
                ImageUrl       = "/images/galaxy-s23-ultra.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Google Pixel 7 128GB",
                Description    = "Obsidian. Heavy scratching on back glass. Screen perfect.",
                Category       = "Phones",
                ConditionGrade = "C",
                Price          = 279.00m,
                StockQuantity  = 15,
                ImageUrl       = "/images/pixel-7.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Apple iPad Air 5th Gen 64GB",
                Description    = "Wi-Fi + Cellular. Starlight. Light surface scratches, no screen damage.",
                Category       = "Tablets",
                ConditionGrade = "B",
                Price          = 449.00m,
                StockQuantity  = 6,
                ImageUrl       = "/images/ipad-air-5.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Samsung Galaxy Tab S8 256GB",
                Description    = "Graphite. Barely used, purchased for a project. S Pen included.",
                Category       = "Tablets",
                ConditionGrade = "A",
                Price          = 549.00m,
                StockQuantity  = 4,
                ImageUrl       = "/images/galaxy-tab-s8.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Sony WH-1000XM5 Headphones",
                Description    = "Black. Noise cancellation fully functional. Ear cushions replaced.",
                Category       = "Accessories",
                ConditionGrade = "B",
                Price          = 229.00m,
                StockQuantity  = 10,
                ImageUrl       = "/images/sony-wh1000xm5.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Apple AirPods Pro (2nd Gen)",
                Description    = "MagSafe case. Right earbud replaced. ANC works perfectly.",
                Category       = "Accessories",
                ConditionGrade = "B",
                Price          = 169.00m,
                StockQuantity  = 9,
                ImageUrl       = "/images/airpods-pro-2.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            },
            new() {
                Name           = "Logitech MX Master 3 Mouse",
                Description    = "Graphite. Minor wear on scroll wheel. All buttons functional.",
                Category       = "Accessories",
                ConditionGrade = "C",
                Price          = 59.00m,
                StockQuantity  = 20,
                ImageUrl       = "/images/mx-master-3.jpg",
                CreatedAt      = DateTime.UtcNow,
                IsActive       = true
            }
        };

        await context.Products.InsertManyAsync(products);
    }
}
