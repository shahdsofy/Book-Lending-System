using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Queries.Models
{
    public class GetBookQuery:IRequest<Response<BookDTO>>
    {
        public int BookId { get; set; }
    }
}
