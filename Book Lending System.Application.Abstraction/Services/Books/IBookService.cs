using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;

namespace Book_Lending_System.Application.Abstraction.Services.Books
{
    public interface IBookService
    {
        Task<Response<IEnumerable<BookDTO>>> GetAllBooksAsync();
        Task<Response<BookDTO>> GetBookByIdAsync(int bookId);
        Task<Response<BookDTO>> AddBookAsync(CreateBookDTO createBookDto);
        Task<Response<BookDTO>> UpdateBookAsync(int Id,UpdateBookDTO updateBookDto);
        Task<Response<bool>> DeleteBookAsync(int bookId);
        

    }
}
