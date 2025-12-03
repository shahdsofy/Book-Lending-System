using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Abstraction.Services.Books;
using Book_Lending_System.Application.Features.Books.Commands.Models;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Commands.Handlers
{
    public class BookCommandHandler : 
        IRequestHandler<AddBookCommand, Response<BookDTO>>,
        IRequestHandler<DeleteBookCommand,Response<bool>>,
        IRequestHandler<UpdateBookCommand,Response<BookDTO>>
    {
        private readonly IBookService bookService;

        public BookCommandHandler(IBookService bookService)
        {
            this.bookService = bookService;
        }

        public async Task<Response<BookDTO>> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            return await bookService.AddBookAsync(request.CreateBookDTO);
        }

        public async Task<Response<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            return await bookService.DeleteBookAsync(request.BookId);
        }

        public async Task<Response<BookDTO>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            return await bookService.UpdateBookAsync(request.BookId,request.UpdateBookDTO);
        }
    }
}
