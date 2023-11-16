using DatabaseServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPrivateManager.Data;
using MyPrivateManager.DatabaseServices;
using MyPrivateManager.IDatabaseServices;
using MyPrivateManager.Models;

var builder = WebApplication.CreateBuilder(args);

//database connection
var connectionString = builder.Configuration.GetConnectionString("DatabaseContext");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<ICategoryServices, CategoryServices>();
builder.Services.AddScoped<IExpenseServices, ExpenseServices>();
builder.Services.AddScoped<IIncomeServices, IncomeServices>();
builder.Services.AddScoped<ISourceServices, SourceServices>();
builder.Services.AddScoped<IUserManager, UserManager>();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<DatabaseContext>();



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

SetRoleOnDatabase.CreateRoleOnDatabase(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
