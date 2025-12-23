using MyPrivateManager.Models;
namespace MyPrivateManager.IDatabaseServices;

public interface IOrderServices
{
    Task<IEnumerable<Order>> GetOrdersAsync();
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<bool> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderAsync(int orderId, Order order);
    Task<bool> DeleteOrderAsync(int orderId);
}
