// ============================================================
// CustomerService.cs — Customer Profile Logic
// ============================================================

using DotShop.API.DTOs.Customers;
using DotShop.API.Services.Interfaces;
using DotShop.Data.Repositories.Interfaces;

namespace DotShop.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;

    public CustomerService(ICustomerRepository repo)
    {
        _repo = repo;
    }

    // -------------------------------------------------------
    // GetByIdAsync — Fetch and map the customer's public profile
    // -------------------------------------------------------
    public async Task<CustomerDto> GetByIdAsync(string id)
    {
        var customer = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer '{id}' not found.");

        // Map to DTO — PasswordHash and Role are deliberately excluded.
        return new CustomerDto(customer.Id, customer.Email,
            customer.FirstName, customer.LastName, customer.CreatedAt);
    }

    // -------------------------------------------------------
    // UpdateAsync — Update name fields only
    // -------------------------------------------------------
    public async Task<bool> UpdateAsync(string id, UpdateCustomerRequest request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;

        // Copy the incoming name changes onto the existing record.
        // All other fields (Email, PasswordHash, Role, CreatedAt) are untouched.
        existing.FirstName = request.FirstName;
        existing.LastName  = request.LastName;

        await _repo.UpdateAsync(id, existing);
        return true;
    }
}
