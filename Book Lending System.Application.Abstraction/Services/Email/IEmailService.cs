using Book_Lending_System.Application.Abstraction.DTOs.Email;

namespace Book_Lending_System.Application.Abstraction.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(email email);
    }
}
