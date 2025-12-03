using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Book_Lending_System.Infrastructure.Persistence.Data.Interceptors;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Book_Lending_System.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceService(this IServiceCollection services,IConfiguration configuration)
        {

            services.AddScoped<CustomSaveChangesInterceptors>();

            services.AddDbContext<StoreDbContext>((sp, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(sp.GetRequiredService<CustomSaveChangesInterceptors>());
            });

            services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddHangfireServer();


            services.AddScoped(typeof(IStoreDbContextInitializer), typeof(StoreDbContextInitializer));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));


           // services.AddScoped(typeof(ISaveChangesInterceptor), typeof(CustomSaveChangesInterceptors));


            return services;
        }
    }
}
