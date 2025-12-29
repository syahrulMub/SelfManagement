using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Models;

namespace MyPrivateManager.Data;

public class DatabaseContext : IdentityDbContext<User>
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Source> Sources { get; set; }
    public DbSet<TaskCategory> TaskCategories { get; set; }
    public DbSet<TaskWork> TaskWorks { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<Technician> Technicians { get; set; } = default!;
    public DbSet<Rating> Ratings { get; set; } = default!;
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {


    }
}
