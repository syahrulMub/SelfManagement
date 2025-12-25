using MyPrivateManager.Models;

namespace MyPrivateManager.DTOs;

// Customer DTOs
public class CreateCustomerDto
{
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}

public class UpdateCustomerDto
{
    public int CustomerId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
}

// Technician DTOs
public class CreateTechnicianDto
{
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UpdateTechnicianDto
{
    public int TechnicianId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
