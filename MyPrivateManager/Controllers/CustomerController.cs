using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

namespace MyPrivateManager.Controllers;

public class CustomerController : Controller
{
    private readonly ICustomerServices _customerServices;
    private readonly ILogger<CustomerController> _logger;
    private readonly UserManager<User> _userManager;

    public CustomerController(ICustomerServices services, ILogger<CustomerController> logger, UserManager<User> userManager)
    {
        _customerServices = services;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet("/Customer/Customers")]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var customers = await _customerServices.GetCustomersAsync();
                var userCustomers = customers.Where(c => c.UserId == userId);
                return Ok(userCustomers);
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return View("Error");
        }
    }

    [HttpGet("/Customer/GetCustomer/{customerId}")]
    public async Task<IActionResult> GetCustomer(int customerId)
    {
        try
        {
            var customer = await _customerServices.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogError("Customer not found");
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved customer data");
            return Ok(customer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer");
            return View("Error");
        }
    }

    [HttpPost("/Customer/CreateCustomer")]
    public async Task<IActionResult> CreateCustomer(Customer customer)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                customer.UserId = userId;
                var success = await _customerServices.CreateCustomerAsync(customer);
                _logger.LogInformation("Successfully created customer");
                return Ok();
            }
            else
            {
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return View("Error");
        }
    }

    [HttpPost("/Customer/UpdateCustomer/{customerId}")]
    public async Task<IActionResult> UpdateCustomer(int customerId, Customer customer)
    {
        try
        {
            await _customerServices.UpdateCustomerAsync(customerId, customer);
            _logger.LogInformation("Successfully updated customer");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return View("Error");
        }
    }

    [HttpDelete("/Customer/DeleteCustomer/{customerId}")]
    public async Task<ActionResult> DeleteCustomer(int customerId)
    {
        try
        {
            await _customerServices.DeleteCustomerAsync(customerId);
            _logger.LogInformation("Successfully deleted customer");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            return View("Error");
        }
    }
}
