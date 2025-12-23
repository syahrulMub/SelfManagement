using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class CustomerServices : ICustomerServices
    {
        private readonly DatabaseContext _dbContext;

        public CustomerServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _dbContext.Customers
                            .Include(c => c.User)
                            .Include(c => c.Orders)
                            .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _dbContext.Customers
                    .Include(c => c.User)
                    .Include(c => c.Orders)
                    .Where(c => c.CustomerId == customerId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCustomerAsync(int customerId, Customer customer)
        {
            var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (existingCustomer != null)
            {
                existingCustomer.UserId = customer.UserId;
                existingCustomer.Latitude = customer.Latitude;
                existingCustomer.Longitude = customer.Longitude;
                existingCustomer.Address = customer.Address;
                existingCustomer.City = customer.City;
                _dbContext.Customers.Update(existingCustomer);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
