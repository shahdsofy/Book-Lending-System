using AutoMapper;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Abstraction.Services.Books;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Shared.Errors;
using Book_Lending_System.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Book_Lending_System.Application.Services.Books
{
    public class BookService(IUnitOfWork unitOfWork, IMapper mapper) : IBookService
    {
        

        public async Task<Response<BookDTO>> AddBookAsync(CreateBookDTO createBookDto)
        {
            var book = mapper.Map<Book>(createBookDto);
            await unitOfWork.GetRepository<Book, int>().AddAsync(book);
            var result = await unitOfWork.CompleteAsync();

            if (result > 0)
            {
                var bookToreturn = mapper.Map<BookDTO>(book);
                return Response<BookDTO>.Success(bookToreturn);
            }

            return Response<BookDTO>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Unable to create book");
        }

        public async Task<Response<bool>> DeleteBookAsync(int bookId)
        {
            var repo =  unitOfWork.GetRepository<Book, int>();
            var book=await repo.GetByIdAsync(bookId);
            if (book == null)
            {
                return Response<bool>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book Not Found");
            }
           
            
            repo.Delete(book);
            var res = await unitOfWork.CompleteAsync();

            if (res > 0)
            {
                await unitOfWork.CompleteAsync();
                return Response<bool>.Success(true, "Book is deleted successfully");
            }
            return Response<bool>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Failed to delete book");
        }

        public  async Task<Response<BookDTO>> UpdateBookAsync(int Id,UpdateBookDTO updateBookDto)
        {
            var repo = unitOfWork.GetRepository<Book, int>();
            var book = await repo.GetByIdAsync(Id);

            if(book == null)
            {
                return Response<BookDTO>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book not found");
            }

          
           
            if (updateBookDto.Title != null)
                   book.Title = updateBookDto.Title;
            if (updateBookDto.Description != null)
                book.Description = updateBookDto.Description;
            if (updateBookDto.Author != null)
                book.Author = updateBookDto.Author;
            

            if(updateBookDto.Title==null && updateBookDto.Description==null && updateBookDto.Author==null)
                 return Response<BookDTO>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Title and Description and Author can not be null.");
                   
            
            repo.Update(book);
            var res = await unitOfWork.CompleteAsync() > 0;
            if (res)
            {
                var bookToreturn = mapper.Map<BookDTO>(book);
                return Response<BookDTO>.Success(bookToreturn);
            }
            return Response<BookDTO>.Fail(HttpStatusCode.BadRequest, ErrorType.BadRequest.ToString(), "Failed to Update Book");

        }

        public Response<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books =  unitOfWork.GetRepository<Book, int>().GetAllAsQuerable()
                                                              .Where(x => x.IsAvailable == true)
                                                              .ToListAsync();//عشان الفلتره تتم في الداتا بيز

            if (books == null)
                return Response<IEnumerable<BookDTO>>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book is not found");

            var booksToReturn = mapper.Map<IEnumerable<BookDTO>>(books);
            return Response<IEnumerable<BookDTO>>.Success(booksToReturn);
        }

        public async Task<Response<BookDTO>> GetBookByIdAsync(int bookId)
        {
            var book = await unitOfWork.GetRepository<Book, int>().GetByIdAsync(bookId);

            if (book == null)
                return Response<BookDTO>.Fail(HttpStatusCode.NotFound, ErrorType.NotFound.ToString(), "Book is not found");


            var returnedBook = mapper.Map<BookDTO>(book);

            return Response<BookDTO>.Success(returnedBook);
        }




    }
}
