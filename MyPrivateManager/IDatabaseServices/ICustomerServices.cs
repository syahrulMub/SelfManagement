using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface ICustomerServices
{
    Task<IEnumerable<Customer>> GetCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int customerId);
    Task<bool> CreateCustomerAsync(Customer customer);
    Task<bool> UpdateCustomerAsync(int customerId, Customer customer);
    Task<bool> DeleteCustomerAsync(int customerId);
}
