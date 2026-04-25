// ============================================================
// ICustomerRepository.cs — Customer Repository Contract
// ============================================================
// Defines the database operations available for Customer documents.
// ============================================================

using DotShop.Data.Models;

namespace DotShop.Data.Repositories.Interfaces;

public interface ICustomerRepository
{
    // Look up a customer by their MongoDB ObjectId.
    // Used when fetching the logged-in user's profile (GET /api/customers/me).
    Task<Customer?> GetByIdAsync(string id);

    // Look up a customer by email address.
    // Used during login to find the account before verifying the password.
    // Also used during registration to check if the email is already taken.
    Task<Customer?> GetByEmailAsync(string email);

    // Insert a new customer document into the "customers" collection.
    // Password must already be BCrypt-hashed by AuthService before calling this.
    Task CreateAsync(Customer customer);

    // Replace the existing customer document with updated profile data.
    // Only non-sensitive fields (FirstName, LastName) should be updatable
    // via the public API — password changes would go through a separate flow.
    Task UpdateAsync(string id, Customer customer);
}
