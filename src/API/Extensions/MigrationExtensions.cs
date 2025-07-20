using Microsoft.EntityFrameworkCore;
using Modules.Accounts.Infrastructure;
using Modules.Users.Infrastructure;

namespace API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using UsersDbContext usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        using AccountsDbContext accountsDbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

        RetryOnFailure(() =>
        {
            usersDbContext.Database.EnsureDeleted(); // Only for development
            usersDbContext.Database.Migrate();

            accountsDbContext.Database.Migrate();
        });
    }

    private static void RetryOnFailure(Action action, int maxRetries = 10, int delaySeconds = 5)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                action();
                break;
            }
            catch (Exception ex)
            {
                attempt++;

                if (attempt > maxRetries)
                {
                    throw;
                }

                Console.WriteLine($"Migration attempt {attempt} failed: {ex.Message}");
                Thread.Sleep(delaySeconds * 1000);
            }
        }
    }
}