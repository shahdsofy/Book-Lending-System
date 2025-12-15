using Book_Lending_System.APIs.Controllers;
using Book_Lending_System.Application.Features.BorrowBook.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Book_Lending_System.Controllers.Controllers.BorrowBook
{
    public class BorrowBookController:BaseController
    {
        private readonly IMediator mediator;

        public BorrowBookController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost("BorrowBook")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> BorrowBook(int BookId)
        {
            var result = await mediator.Send(new BorrowBookCommand { BookId= BookId });
            return StatusCode((int)result.StatusCode,result);
        }

        [HttpPost("ReturnBook")]
        [Authorize(Roles = "Member")]
        public  async Task<IActionResult> ReturnBook(int BookId)
        {
            var result = await mediator.Send(new ReturnBookCommand { BookId = BookId });
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
