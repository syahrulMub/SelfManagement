using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyPrivateManager.DTOs;
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

    // ===== ADMIN MASTER DATA MANAGEMENT =====

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Customers")]
    public IActionResult MasterData()
    {
        return View("~/Views/MasterData/Customers.cshtml");
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Customers/GetData")]
    public async Task<IActionResult> GetCustomersData()
    {
        try
        {
            var customers = await _customerServices.GetCustomersAsync();
            var customerDataList = customers.Select(c => new
            {
                customerId = c.CustomerId,
                userId = c.UserId,
                userEmail = c.User?.Email ?? "N/A",
                address = c.Address,
                city = c.City,
                latitude = c.Latitude,
                longitude = c.Longitude,
                orderCount = c.Orders?.Count ?? 0
            });

            return Json(new { data = customerDataList });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers data");
            return StatusCode(500, new { error = "Error retrieving customers data" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Customers/Get/{customerId}")]
    public async Task<IActionResult> GetCustomerById(int customerId)
    {
        try
        {
            var customer = await _customerServices.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            return Json(new
            {
                customerId = customer.CustomerId,
                userId = customer.UserId,
                userEmail = customer.User?.Email ?? "N/A",
                address = customer.Address,
                city = customer.City,
                latitude = customer.Latitude,
                longitude = customer.Longitude
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer");
            return StatusCode(500, new { error = "Error retrieving customer" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("/Admin/Customers/Create")]
    public async Task<IActionResult> CreateCustomerAdmin([FromBody] CreateCustomerDto dto)
    {
        try
        {
            // Get the user
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Update user phone if provided
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            // Check if customer already exists for this user
            var customers = await _customerServices.GetCustomersAsync();
            if (customers.Any(c => c.UserId == dto.UserId))
            {
                return BadRequest(new { error = "Customer profile already exists for this user" });
            }

            // Create customer
            var customer = new Customer
            {
                UserId = dto.UserId,
                Address = dto.Address,
                City = dto.City,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            await _customerServices.CreateCustomerAsync(customer);
            _logger.LogInformation($"Successfully created customer for user {user.Email}");
            return Ok(new { message = "Customer created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, new { error = "Error creating customer" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("/Admin/Customers/Update/{customerId}")]
    public async Task<IActionResult> UpdateCustomerAdmin(int customerId, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            // Get existing customer
            var customer = await _customerServices.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound(new { error = "Customer not found" });
            }

            // Update user phone if provided
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                var user = await _userManager.FindByIdAsync(customer.UserId);
                if (user != null)
                {
                    user.PhoneNumber = dto.PhoneNumber;
                    await _userManager.UpdateAsync(user);
                }
            }

            // Update customer profile
            customer.Address = dto.Address;
            customer.City = dto.City;
            customer.Latitude = dto.Latitude;
            customer.Longitude = dto.Longitude;

            var success = await _customerServices.UpdateCustomerAsync(customerId, customer);
            if (!success)
            {
                return NotFound(new { error = "Customer not found" });
            }

            _logger.LogInformation($"Successfully updated customer {customerId}");
            return Ok(new { message = "Customer updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return StatusCode(500, new { error = "Error updating customer" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("/Admin/Customers/Delete/{customerId}")]
    public async Task<IActionResult> DeleteCustomerAdmin(int customerId)
    {
        try
        {
            var success = await _customerServices.DeleteCustomerAsync(customerId);
            if (!success)
            {
                return NotFound(new { error = "Customer not found" });
            }
            _logger.LogInformation("Successfully deleted customer (admin)");
            return Ok(new { message = "Customer deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            return StatusCode(500, new { error = "Error deleting customer" });
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("/Admin/Customers/GetAvailableUsers")]
    public async Task<IActionResult> GetAvailableUsers()
    {
        try
        {
            // Get all users
            var users = _userManager.Users.ToList();
            
            // Get users already assigned as customers
            var customers = await _customerServices.GetCustomersAsync();
            var assignedUserIds = customers.Select(c => c.UserId).ToHashSet();
            
            // Filter out assigned users
            var availableUsers = users
                .Where(u => !assignedUserIds.Contains(u.Id))
                .Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    userName = u.UserName,
                    phoneNumber = u.PhoneNumber
                })
                .ToList();
            
            return Json(new { data = availableUsers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available users");
            return StatusCode(500, new { error = "Error retrieving available users" });
        }
    }
}
