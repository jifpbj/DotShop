using System.ComponentModel.DataAnnotations;

namespace DotShop.API.DTOs.Customers;

// -------------------------------------------------------
// UpdateCustomerRequest — Body of PUT /api/customers/me
// -------------------------------------------------------
// Customers can only update their own name — not their email,
// password, or role. Those require dedicated, verified flows.
public record UpdateCustomerRequest(
    [Required] string FirstName,
    [Required] string LastName
);
