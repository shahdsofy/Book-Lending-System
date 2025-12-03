using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Shared.Responses;
using MediatR;

namespace Book_Lending_System.Application.Features.Books.Queries.Models
{
    public class GetAllQuery:IRequest<Response<IEnumerable<BookDTO>>>
    {
    }
}
