using Book_Lending_System.Application.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Book_Lending_System.Shared.Responses;

namespace Book_Lending_System.APIs.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = false;
                    options.InvalidModelStateResponseFactory = (actionContext =>
                    {
                        var Errors = actionContext.ModelState.Where(e => e.Value!.Errors.Count() > 0)
                                                    .SelectMany(e => e.Value!.Errors).Select(e => e.ErrorMessage);

                        return new BadRequestObjectResult(new Response<object>() { Errors = Errors.ToList() });

                    });
                }
            )
                // Allow to see Controllers in another Assembly (Arabeya.APIs.Controllers)
                .AddApplicationPart(typeof(AssemblyInformation).Assembly);




            services.AddScoped(typeof(ILoggedInUserService),typeof(LoggedInUserService));
            return services;
        }
    }
}
