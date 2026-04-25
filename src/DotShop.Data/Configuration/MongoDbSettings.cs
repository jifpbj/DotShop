// ============================================================
// MongoDbSettings.cs — Strongly-Typed MongoDB Configuration
// ============================================================
// This class is the C# mirror of the "MongoDbSettings" section
// in appsettings.json. It lets us read config values as a typed
// object rather than raw strings.
//
// HOW IT WORKS:
//   1. In appsettings.json you define:
//        "MongoDbSettings": {
//          "ConnectionString": "mongodb://localhost:27017",
//          "DatabaseName": "DotShopDb"
//        }
//   2. In Program.cs you bind it:
//        builder.Services.Configure<MongoDbSettings>(
//            builder.Configuration.GetSection("MongoDbSettings"));
//   3. Any class that needs these values asks for IOptions<MongoDbSettings>
//      via dependency injection — .NET fills it in automatically.
//
// WHY NOT just call Configuration["MongoDbSettings:ConnectionString"]?
//   Magic strings are hard to refactor and easy to typo.
//   A strongly-typed class gives you compile-time safety and IDE autocomplete.
// ============================================================

namespace DotShop.Data.Configuration;

public class MongoDbSettings
{
    // The MongoDB connection string. Format: mongodb://host:port
    // For local development: "mongodb://localhost:27017"
    // For Atlas (cloud): "mongodb+srv://user:pass@cluster.mongodb.net"
    public string ConnectionString { get; set; } = null!;

    // The name of the database inside MongoDB to use.
    // MongoDB creates the database automatically on first write if it doesn't exist.
    public string DatabaseName { get; set; } = null!;
}
