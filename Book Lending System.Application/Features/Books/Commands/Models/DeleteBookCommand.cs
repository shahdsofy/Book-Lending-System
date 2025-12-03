using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Commands.Models
{
    public class DeleteBookCommand:IRequest<Response<bool>>
    {
        public required int BookId { get; set; }
    }
}
