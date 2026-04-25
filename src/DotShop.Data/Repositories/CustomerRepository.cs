// ============================================================
// CustomerRepository.cs — MongoDB Queries for Customers
// ============================================================
// Implements ICustomerRepository using the MongoDB .NET Driver.
// ============================================================

using DotShop.Data.Models;
using DotShop.Data.Repositories.Interfaces;
using MongoDB.Driver;

namespace DotShop.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomerRepository(MongoDbContext context)
    {
        _customers = context.Customers;
    }

    // -------------------------------------------------------
    // GetByIdAsync — Fetch a customer profile by their ObjectId
    // -------------------------------------------------------
    public async Task<Customer?> GetByIdAsync(string id)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        return await _customers.Find(filter).FirstOrDefaultAsync();
    }

    // -------------------------------------------------------
    // GetByEmailAsync — Look up a customer by email
    // -------------------------------------------------------
    // Used by AuthService during login: find the account, then
    // BCrypt.Verify(plainPassword, customer.PasswordHash).
    // Case-insensitive comparison avoids "User@Example.com" vs
    // "user@example.com" being treated as two different accounts.
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        // Regex filter with IgnoreCase option performs a case-insensitive match.
        var filter = Builders<Customer>.Filter
            .Regex(c => c.Email, new MongoDB.Bson.BsonRegularExpression($"^{email}$", "i"));

        return await _customers.Find(filter).FirstOrDefaultAsync();
    }

    // -------------------------------------------------------
    // CreateAsync — Register a new customer
    // -------------------------------------------------------
    // AuthService hashes the password with BCrypt before calling this.
    public async Task CreateAsync(Customer customer)
    {
        await _customers.InsertOneAsync(customer);
    }

    // -------------------------------------------------------
    // UpdateAsync — Update customer profile fields
    // -------------------------------------------------------
    // Replaces the entire document. The caller (CustomerService)
    // is responsible for preserving fields that shouldn't change
    // (e.g. PasswordHash, Role, CreatedAt) by copying them from
    // the existing record before calling this method.
    public async Task UpdateAsync(string id, Customer customer)
    {
        var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
        await _customers.ReplaceOneAsync(filter, customer);
    }
}
