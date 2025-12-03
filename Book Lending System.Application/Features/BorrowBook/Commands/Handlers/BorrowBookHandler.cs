using Book_Lending_System.Application.Abstraction.Services.BorrowBook;
using Book_Lending_System.Application.Features.BorrowBook.Commands.Models;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.BorrowBook.Commands.Handlers
{
    public class BorrowBookHandler :
        IRequestHandler<BorrowBookCommand, Response<string>>,
        IRequestHandler<ReturnBookCommand, Response<string>>
    {
        private readonly IBorrowBookService borrowBookService;

        public BorrowBookHandler(IBorrowBookService borrowBookService)
        {
            this.borrowBookService = borrowBookService;
        }
        public async Task<Response<string>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            return await borrowBookService.BorrowBookAsync(request.BookId);
        }

        public async Task<Response<string>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            return await borrowBookService.ReturnBookAsync(request.BookId);
        }
    }
}
