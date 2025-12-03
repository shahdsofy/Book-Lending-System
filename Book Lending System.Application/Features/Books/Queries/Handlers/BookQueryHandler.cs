using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Abstraction.Services.Books;
using Book_Lending_System.Application.Features.Books.Queries.Models;
using Book_Lending_System.Shared.Responses;
using MediatR;
using System.Threading.Tasks;

namespace Book_Lending_System.Application.Features.Books.Queries.Handlers
{
    public class BookQueryHandler : 
        IRequestHandler<GetAllQuery, Response<IEnumerable<BookDTO>>>,
        IRequestHandler<GetBookQuery,Response<BookDTO>>
    {
        private readonly IBookService bookService;

        public BookQueryHandler(IBookService bookService)
        {
            this.bookService = bookService;
        }
        public Response<IEnumerable<BookDTO>> Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            return  bookService.GetAllBooksAsync();
        }

        public async Task<Response<BookDTO>> Handle(GetBookQuery request, CancellationToken cancellationToken)
        {
            return await bookService.GetBookByIdAsync(request.BookId);
        }

        async Task<Response<IEnumerable<BookDTO>>> IRequestHandler<GetAllQuery, Response<IEnumerable<BookDTO>>>.Handle(GetAllQuery request, CancellationToken cancellationToken)
        {
            return  bookService.GetAllBooksAsync();
        }
    }
}
