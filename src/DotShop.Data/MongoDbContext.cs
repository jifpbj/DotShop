// ============================================================
// MongoDbContext.cs — The Database Connection Manager
// ============================================================
// This class is the single place that owns the connection to MongoDB
// and hands out references to each collection (table equivalent).
//
// It is registered as a SINGLETON in the DI container (Program.cs),
// meaning only ONE instance is created for the entire lifetime of the app.
//
// WHY SINGLETON?
//   MongoClient (the low-level MongoDB connection) is thread-safe and
//   manages an internal connection pool — it's designed to be shared
//   across all requests. Creating a new MongoClient on every request
//   would be wasteful and slow (re-establishing connections each time).
//
// HOW IT FITS IN:
//   Program.cs registers MongoDbContext as a singleton.
//   Repositories receive it via constructor injection.
//   Each repository then calls .Products / .Orders / .Customers
//   to get a typed collection reference and run queries against it.
// ============================================================

using DotShop.Data.Configuration;
using DotShop.Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DotShop.Data;

public class MongoDbContext
{
    // The internal database reference — private so only this class
    // can access the raw IMongoDatabase handle.
    private readonly IMongoDatabase _database;

    // IOptions<T> is .NET's way of injecting configuration objects.
    // When Program.cs calls builder.Services.Configure<MongoDbSettings>(...),
    // .NET makes the settings available here automatically.
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        // MongoClient manages the connection pool to the MongoDB server.
        // Passing the connection string is all it needs to connect.
        var client = new MongoClient(settings.Value.ConnectionString);

        // GetDatabase returns (or creates) the named database.
        // MongoDB creates the database lazily — it won't actually appear
        // until the first document is written to it.
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    // Each property below returns an IMongoCollection<T> — the equivalent
    // of a SQL table, but for MongoDB documents. Repositories use these
    // to run Find, InsertOne, UpdateOne, DeleteOne, etc.
    //
    // These are lightweight property calls — NOT new connections.
    // They simply return a reference to the named collection within the database.

    // The "products" collection stores all Product documents.
    public IMongoCollection<Product> Products =>
        _database.GetCollection<Product>("products");

    // The "orders" collection stores all Order documents (with embedded OrderItems).
    public IMongoCollection<Order> Orders =>
        _database.GetCollection<Order>("orders");

    // The "customers" collection stores all Customer documents.
    public IMongoCollection<Customer> Customers =>
        _database.GetCollection<Customer>("customers");
}
