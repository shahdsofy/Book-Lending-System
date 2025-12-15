
using Book_Lending_System.APIs.Extensions;
using Book_Lending_System.APIs.Middlewares;
using Book_Lending_System.Application;
using Book_Lending_System.Application.Services.BorrowBook;
using Book_Lending_System.Infrastructure.Persistence;
using Hangfire;
using System.Threading.Tasks;

namespace Book_Lending_System.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddIdentityService(builder.Configuration);
            builder.Services.AddPersistenceService(builder.Configuration);
            builder.Services.AddAPIServices();
            builder.Services.AddApplicationServices();

          
            var app = builder.Build();

           

            await app.InitializeDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/Dashboard");

            app.MapControllers();

            app.Run();
        }
    }
}
