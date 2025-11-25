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
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }
}
