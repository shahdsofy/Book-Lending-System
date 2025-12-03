using AutoMapper;
using Book_Lending_System.Application.Abstraction;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Abstraction.Services.BorrowBook;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Shared.Errors;
using Book_Lending_System.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Book_Lending_System.Application.Services.BorrowBook
{
    public class BorrowBookService(IUnitOfWork unitOfWork, ILoggedInUserService loggedInUserService, IMapper mapper) : IBorrowBookService
    {
        public async Task<Response<string>> BorrowBookAsync(int BookId)
        {
            var userid = loggedInUserService.UserId;

            var repo1 = unitOfWork.GetRepository<BorrowRecord, int>();
            var repo = unitOfWork.GetRepository<Book, int>();


            var book = await repo.GetByIdAsync(BookId);
            if (book == null)
                return Response<string>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book is not found");
            if (!book.IsAvailable)
                return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Book is not available for borrowing.");

            //عشان اعرف لو اليوزر عنده كتاب في الوقت الحالي
            var HasActivBorrowing = await repo1.GetAllAsQuerable()
                .AnyAsync(x => x.UserId == userid && x.ReturnedAt == null);
            if (HasActivBorrowing)
                return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "You can only borrow one book at a time.");


            var Borrowrecord = new Abstraction.DTOs.Books.BorrowBook
            {
                UserId = userid,
                BookId = BookId,
                Borrowedat = DateTime.Now
            };
            var SavedBorrowrecord = mapper.Map<BorrowRecord>(Borrowrecord);

            SavedBorrowrecord.CreatedBy = userid;
            SavedBorrowrecord.LastModifiedBy = userid;

            book.IsAvailable = false;
            repo.Update(book);

            await repo1.AddAsync(SavedBorrowrecord);
            var result = await unitOfWork.CompleteAsync() > 0;
            if (result)
                return Response<string>.Success("Book borrowed successfully.");
            return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Failed to borrow book");


        }

        public async Task<Response<string>> ReturnBookAsync(int BookId)
        {
            var userid = loggedInUserService.UserId;

            var repo1 = unitOfWork.GetRepository<BorrowRecord, int>();
            var repo = unitOfWork.GetRepository<Book, int>();

            var CheckBorrowdBook =await repo1.GetAllAsQuerable()
                .FirstOrDefaultAsync(x => x.BookId == BookId && x.UserId == userid && x.ReturnedAt == null)
                ;

            if (CheckBorrowdBook == null)
                return Response<string>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "No active borrowing record found for this book.");

            CheckBorrowdBook.ReturnedAt= DateTime.Now;


            var book = await repo.GetByIdAsync(BookId);
            if (book == null)
                return Response<string>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book not found");

            book.IsAvailable = true;
            repo.Update(book);

            var result = await unitOfWork.CompleteAsync() > 0;

            if (result)
                return Response<string>.Success("Book returned successfully.");

            return Response<string>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Failed to return book.");
        }
    }
}
