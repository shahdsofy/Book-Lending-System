using Book_Lending_System.Application.Abstraction.DTOs.Email;
using Book_Lending_System.Application.Abstraction.Services.BorrowBook;
using Book_Lending_System.Application.Abstraction.Services.Email;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Book_Lending_System.Application.Services.BorrowBook
{
    public class OverdueBooksService : IOverdueBooksService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<OverdueBooksService> logger;
        private readonly IEmailService emailService;

        public OverdueBooksService(IUnitOfWork unitOfWork,
            ILogger<OverdueBooksService> logger, IEmailService emailService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.emailService = emailService;
        }
        public async Task ProcessOverdueBooksAsync()
        {
            logger.LogInformation($"OverdueChecker started at {DateTime.Now}");

            //هيجيب اللي عدا عليهم 7 ايام او اكتر
            //var overdueBooks = await dbContext.BorrowRecords.Include(x => x.User).Include(x => x.Book)
            //    .Where(x => x.BorrowedAt.AddDays(7) < DateTime.Now).ToListAsync();

            var overdueBooks = await unitOfWork.GetRepository<BorrowRecord,int>().GetAllAsQuerable()
                .Include(x => x.User).Include(x => x.Book)
               .Where(x => x.BorrowedAt.AddDays(7) < DateTime.Now).ToListAsync();

            if (!overdueBooks.Any())
                logger.LogInformation("No overdue book found");

            foreach (var overdueBook in overdueBooks)
            {
                var toEmail = overdueBook.User?.Email ?? "(no-email)";
                var subject = $"Overdue book reminder: {overdueBook.Book?.Title ?? "Unknown"}";
                var body = $@"Hello {overdueBook.User?.UserName ?? "Member"},
                  Your borrowed book '{overdueBook.Book?.Title ?? "Unknown"}' was due on {overdueBook.BorrowedAt.AddDays(7):u}.
                  Please return it as soon as possible.";

                var email = new email
                {
                    To = toEmail,
                    Subject = subject,
                    Body = body
                };
                logger.LogInformation("Found overdue: BorrowRecordId={Id}, BookId={BookId}, UserId={UserId}",
                    overdueBook.Id, overdueBook.BookId, overdueBook.UserId);

                try
                {
                    await emailService.SendEmailAsync(email);
                    logger.LogInformation("Reminder sent for BorrowRecordId={Id}", overdueBook.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send reminder for BorrowRecordId={Id}", overdueBook.Id);
                }
            }

            logger.LogInformation("OverdueChecker finished at {Now}", DateTime.UtcNow);
        }
    }

}