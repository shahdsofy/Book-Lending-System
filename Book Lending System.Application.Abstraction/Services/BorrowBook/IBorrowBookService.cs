
using Book_Lending_System.Shared.Responses;

namespace Book_Lending_System.Application.Abstraction.Services.BorrowBook
{
    public interface IBorrowBookService
    {
        Task<Response<string>>BorrowBookAsync(int BookId);
        Task<Response<string>>ReturnBookAsync (int BookId);
    }
}
