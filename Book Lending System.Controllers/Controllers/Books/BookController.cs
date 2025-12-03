using Book_Lending_System.APIs.Controllers;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Abstraction.Services.BorrowBook;
using Book_Lending_System.Application.Features.Books.Commands.Models;
using Book_Lending_System.Application.Features.Books.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Book_Lending_System.Controllers.Controllers.Books
{
    public class BookController:BaseController
    {
        private readonly IMediator mediator;

        public BookController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("AddBook")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddBook (CreateBookDTO bookDTO)
        {
            var result= await mediator.Send(new AddBookCommand { CreateBookDTO =bookDTO});
            return StatusCode((int)result.StatusCode, result);

        }
        [HttpDelete("DeleteBook")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook (int id)
        {
            var result = await mediator.Send(new DeleteBookCommand { BookId = id });
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("UpdateBook")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int Id,UpdateBookDTO bookDTO)
        {
            var result = await mediator.Send(new UpdateBookCommand { UpdateBookDTO = bookDTO,BookId=Id });
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllBooks()
        {
            var result = mediator.Send(new GetAllQuery());
            return Ok(result);
        }
        [HttpGet("GetBook")]
        public async Task<IActionResult> GetBook(int id)
        {
            var result=await mediator.Send(new GetBookQuery { BookId = id });
            return StatusCode((int)result.StatusCode, result);
        }

       
    }
}
