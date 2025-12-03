using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Commands.Models
{
    public class AddBookCommand:IRequest<Response<BookDTO>>
    {
        public  CreateBookDTO CreateBookDTO { get; set; }
    }
}
