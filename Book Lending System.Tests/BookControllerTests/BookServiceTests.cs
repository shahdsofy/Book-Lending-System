using AutoMapper;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Mapping;
using Book_Lending_System.Application.Services.Books;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Book_Lending_System.Tests.BookControllerTests
{
    public class BookServiceTests
    {

        private BookService CreateService(StoreDbContext context)
        {
            //unit of work
            var unitOfWork = new UnitOfWork(context);

            //mapper
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, loggerFactory).CreateMapper();

            return new BookService(unitOfWork, mapper);

        }

        private StoreDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<StoreDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new StoreDbContext(options);
        }


        [Fact]
        public async Task AddBookAsync_ShouldAddBookSuccessfully()
        {
            using var context = CreateInMemoryDb();
            var book = new CreateBookDTO
            {
                Title = "The Pragmatic Programmer",
                Author = "Andrew Hunt and David Thomas",
                Description = "Your Journey to Mastery",
                CreatedBy= "Tester",
                LastModifiedBy= "Tester"
            };

            var service = CreateService(context);
            var result = await service.AddBookAsync(book);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("The Pragmatic Programmer", result.Data.Title);
            Assert.Single(context.Books);



        }

        [Fact]
        public async Task DeleteBookAsync_ShouldFail_WhenBookNotFound()
        {
            using var context = CreateInMemoryDb();
            var service = CreateService(context);

            var result = await service.DeleteBookAsync(10);

            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
        [Fact]
        public async Task DeleteBookAsync_ShouldDeleteBook()
        {
            using var context = CreateInMemoryDb();

            context.Books.Add(new Book
            {
                Id = 1,
                Title = "Test Book",
                Author = "Test Author",
                Description = "Test Description",
                IsAvailable = true,
                CreatedBy= "Tester",
                LastModifiedBy= "Tester"

            });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.DeleteBookAsync(1);

            Assert.True(result.Succeeded);
            Assert.Empty(context.Books);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldFail_WhenAllFieldsNull()
        {
            using var context = CreateInMemoryDb();

            context.Books.Add(new Book { Id = 1, Title = "Old",
                 Author = "Test Author",
                Description = "Test Description"
                ,CreatedBy="Tester",
                LastModifiedBy= "Tester"
            });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateBookDTO();

            var result = await service.UpdateBookAsync(1, dto);

            Assert.False(result.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldUpdateBook()
        {
            using var context = CreateInMemoryDb();

            context.Books.Add(new Book
            {
                Id = 1,
                Title = "Old Title",
                Author = "Old Author",
                Description= "Old Description",
                CreatedBy= "Tester",
                LastModifiedBy= "Tester"

            });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var dto = new UpdateBookDTO
            {
                Title = "New Title"
            };

            var result = await service.UpdateBookAsync(1, dto);

            Assert.True(result.Succeeded);
            Assert.Equal("New Title", result.Data.Title);
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook()
        {
            using var context = CreateInMemoryDb();

            context.Books.Add(new Book { Id = 1, Title = "Book 1",
                Author = "Test Author",
                Description = "Test Description",
                CreatedBy= "Tester",
                LastModifiedBy= "Tester"
            });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            var result = await service.GetBookByIdAsync(1);

            Assert.True(result.Succeeded);
            Assert.Equal("Book 1", result.Data.Title);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnOnlyAvailableBooks()
        {
            using var context = CreateInMemoryDb();

            // Arrange: إضافة كتب
            context.Books.AddRange(
                new Book { Id = 1, Title = "Book 1", IsAvailable = true , Author = "Test Author",
                    Description = "Test Description",
                    CreatedBy= "Tester",
                    LastModifiedBy= "Tester"
                },
                new Book { Id = 2, Title = "Book 2", IsAvailable = false,
                    Author = "Test Author",
                    Description = "Test Description",
                    CreatedBy= "Tester",
                    LastModifiedBy= "Tester"
                },
                new Book { Id = 3, Title = "Book 3", IsAvailable = true,
                    Author = "Test Author",
                    Description = "Test Description",
                    CreatedBy= "Tester",
                    LastModifiedBy= "Tester"
                }
            );
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // Act
            var result = await service.GetAllBooksAsync();

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var books = result.Data.ToList();
            Assert.Equal(2, books.Count); // بس الكتب المتاحة
            Assert.Contains(books, b => b.Title == "Book 1");
            Assert.Contains(books, b => b.Title == "Book 3");
            Assert.DoesNotContain(books, b => b.Title == "Book 2");
        }

     




    }
}
