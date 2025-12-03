using Book_Lending_System.Application.Services.BorrowBook;
using Book_Lending_System.Core.Contracts.Persistence;
using Hangfire;

namespace Book_Lending_System.APIs.Extensions
{
    public static class InitializeExtension
    {
        public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IStoreDbContextInitializer>();

                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

                var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                
                try
                {
                    recurringJobs.AddOrUpdate<OverdueBooksService>("check-overdue-books",
                    job => job.ProcessOverdueBooksAsync(),
                    Cron.Daily);

                    await initializer.InitializeAsync();
                    await initializer.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An Error Accurred During Apply Migrations");
                }
            }
           

            return app;
        }
    }
}
