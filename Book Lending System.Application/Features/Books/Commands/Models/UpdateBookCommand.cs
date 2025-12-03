using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Commands.Models
{
    public class UpdateBookCommand:IRequest<Response<BookDTO>>
    {
        public int BookId { get; set; }
        public UpdateBookDTO UpdateBookDTO { get; set; }
    }
}
