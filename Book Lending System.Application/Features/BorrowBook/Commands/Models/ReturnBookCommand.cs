using Book_Lending_System.Shared.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Lending_System.Application.Features.BorrowBook.Commands.Models
{
    public class ReturnBookCommand:IRequest<Response<string>>
    {
        public int BookId { get; set; }
    }
}
