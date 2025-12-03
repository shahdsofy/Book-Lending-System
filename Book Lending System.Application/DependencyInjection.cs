using Book_Lending_System.Application.Abstraction.Services.Books;
using Book_Lending_System.Application.Abstraction.Services.BorrowBook;
using Book_Lending_System.Application.Abstraction.Services.Email;
using Book_Lending_System.Application.Abstraction.Services.Identity;
using Book_Lending_System.Application.Features.Behaviors;
using Book_Lending_System.Application.Features.Books.Commands.Validators;
using Book_Lending_System.Application.Features.Identity.Commands.Validators;
using Book_Lending_System.Application.Mapping;
using Book_Lending_System.Application.Services.Books;
using Book_Lending_System.Application.Services.BorrowBook;
using Book_Lending_System.Application.Services.Email;
using Book_Lending_System.Application.Services.Identity;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Book_Lending_System.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEmailService),typeof(EmailService));
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddScoped(typeof(IBookService), typeof(BookService));
            services.AddScoped(typeof(IBorrowBookService), typeof(BorrowBookService));
            services.AddScoped(typeof(IOverdueBooksService), typeof(OverdueBooksService));
          
            services.AddMediatR(cfg=> cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddValidatorsFromAssembly(typeof(RegisterValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(LoginValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(AddBookValidator).Assembly);




            return services;
        }
    }
}
