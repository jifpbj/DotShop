using DotShop.API.DTOs.Customers;

namespace DotShop.API.Services.Interfaces;

public interface ICustomerService
{
    // Fetches the authenticated customer's profile by their ID.
    // Throws KeyNotFoundException if the ID doesn't match any customer.
    Task<CustomerDto> GetByIdAsync(string id);

    // Updates FirstName and LastName only.
    // Returns false if the customer was not found.
    Task<bool> UpdateAsync(string id, UpdateCustomerRequest request);
}
