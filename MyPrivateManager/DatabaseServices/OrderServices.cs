using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace DatabaseServices
{
    public class OrderServices : IOrderServices
    {
        private readonly DatabaseContext _dbContext;

        public OrderServices(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _dbContext.Orders
                            .Include(o => o.Customer)
                            .Include(o => o.Technician)
                            .Include(o => o.Ratings)
                            .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _dbContext.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Technician)
                    .Include(o => o.Ratings)
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateOrderAsync(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderAsync(int orderId, Order order)
        {
            var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (existingOrder != null)
            {
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.TechnicianId = order.TechnicianId;
                existingOrder.ScheduledAt = order.ScheduledAt;
                existingOrder.Status = order.Status;
                existingOrder.Price = order.Price;
                existingOrder.PaymentStatus = order.PaymentStatus;
                _dbContext.Orders.Update(existingOrder);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order != null)
            {
                _dbContext.Orders.Remove(order);
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
