using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
namespace MyPrivateManager.Data;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(IApplicationBuilder app, string DatabaseConfig)
    {
        try
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                DatabaseContext? context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
                var dbExists = context != null && ((context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)!).Exists();
                if (!dbExists)
                {
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var dbDirectory = Path.Combine(currentDirectory, "Database");
                    Directory.CreateDirectory(dbDirectory);
                    if (context.Database.EnsureCreated())
                    {
                        context.Database.ExecuteSqlRaw(File.ReadAllText(DatabaseConfig ?? string.Empty));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
