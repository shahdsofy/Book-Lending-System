using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.DTOs.Identity;
using Book_Lending_System.Core.Entities.Identity;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Book_Lending_System.APIs.Extensions
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services,IConfiguration configuration)
        {

            services.Configure<jwtSettings>(configuration.GetSection("jwtSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail=true;

                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase= true;
                options.Password.RequireLowercase= true;

                options.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<StoreDbContext>()
              .AddDefaultTokenProviders();

            // إضافة Authentication مع Default Scheme
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["jwtSettings:Issuer"],
                    ValidAudience = configuration["jwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["jwtSettings:Key"])
                    )
                };
            });

            return services;
        }
    }
}
