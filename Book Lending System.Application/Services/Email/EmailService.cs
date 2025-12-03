using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.Services.Email;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Lending_System.Application.Services.Email
{
    public class EmailService(IOptions<EmailSettings> mailSettings) :IEmailService
    {
        private readonly EmailSettings _mailSettings =mailSettings.Value;

       
        public async Task SendEmailAsync(email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Email),
                Subject = email.Subject,
            };

            mail.To.Add(MailboxAddress.Parse(email.To));
            mail.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));


            var builder = new BodyBuilder();
            builder.TextBody = email.Body;


            mail.Body = builder.ToMessageBody();


            using var smtp = new MailKit.Net.Smtp.SmtpClient();



            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
            await smtp.SendAsync(mail);
            await smtp.DisconnectAsync(true);

        }
    }
}
