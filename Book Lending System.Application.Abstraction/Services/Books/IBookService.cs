using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;

namespace Book_Lending_System.Application.Abstraction.Services.Books
{
    public interface IBookService
    {
        Response<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<Response<BookDTO>> GetBookByIdAsync(int bookId);
        Task<Response<BookDTO>> AddBookAsync(CreateBookDTO createBookDto);
        Task<Response<BookDTO>> UpdateBookAsync(int Id,UpdateBookDTO updateBookDto);
        Task<Response<bool>> DeleteBookAsync(int bookId);
        //Task<BorrowRecordDto> BorrowBookAsync(int bookId, string userId);
        //Task<BorrowRecordDto> ReturnBookAsync(int bookId, string userId);

    }
}
