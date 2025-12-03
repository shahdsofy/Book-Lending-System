using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.BorrowBook.Commands.Models
{
    public class BorrowBookCommand:IRequest<Response<string>>
    {
        public int BookId { get; set; }
    }
}
