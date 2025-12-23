using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class OrderController : Controller
{
    private readonly IOrderServices _orderServices;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderServices services, ILogger<OrderController> logger)
    {
        _orderServices = services;
        _logger = logger;
    }

    [HttpGet("/Order/Orders")]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            var orders = await _orderServices.GetOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return View("Error");
        }
    }

    [HttpGet("/Order/GetOrder/{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        try
        {
            var order = await _orderServices.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogError("Order not found");
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved order data");
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return View("Error");
        }
    }

    [HttpPost("/Order/CreateOrder")]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        try
        {
            var success = await _orderServices.CreateOrderAsync(order);
            _logger.LogInformation("Successfully created order");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return View("Error");
        }
    }

    [HttpPost("/Order/UpdateOrder/{orderId}")]
    public async Task<IActionResult> UpdateOrder(int orderId, Order order)
    {
        try
        {
            await _orderServices.UpdateOrderAsync(orderId, order);
            _logger.LogInformation("Successfully updated order");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order");
            return View("Error");
        }
    }

    [HttpDelete("/Order/DeleteOrder/{orderId}")]
    public async Task<ActionResult> DeleteOrder(int orderId)
    {
        try
        {
            await _orderServices.DeleteOrderAsync(orderId);
            _logger.LogInformation("Successfully deleted order");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order");
            return View("Error");
        }
    }
}
