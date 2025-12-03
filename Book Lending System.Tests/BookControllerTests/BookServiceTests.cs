using AutoMapper;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Application.Services.Books;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using NSubstitute;
using System.Net;

namespace Book_Lending_System.Tests.BookControllerTests
{
    public class BookServiceTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();

        private BookService CreateService()
            => new BookService(_unitOfWork, _mapper);

        [Fact]
        public async Task AddBookAsync_ReturnsSuccess_When_CompleteGreaterThanZero()
        {
            // Arrange
            var create = new CreateBookDTO { Title = "T", Author = "A", Description = "D" };
            var book = new Book { Id = 1, Title = create.Title, Author = create.Author, Description = create.Description, IsAvailable = true };
            var bookDto = new BookDTO { Id = 1, Title = book.Title, Author = book.Author, Description = book.Description, IsAvailable = true };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);

            // mapper: CreateBookDTO -> Book, and Book -> BookDTO
            _mapper.Map<Book>(create).Returns(book);
            _mapper.Map<BookDTO>(book).Returns(bookDto);

            // AddAsync is called (no-op) and CompleteAsync returns 1
             repo.ReceivedCalls(); // keep NSubstitute happy about usage
            _unitOfWork.CompleteAsync().Returns(1);

            var svc = CreateService();

            // Act
            var res = await svc.AddBookAsync(create);

            // Assert
            Assert.True(res.Succeeded);
            Assert.NotNull(res.Data);
            Assert.Equal(bookDto.Id, res.Data!.Id);
            Assert.Equal(bookDto.Title, res.Data.Title);
        }

        [Fact]
        public async Task AddBookAsync_ReturnsFail_When_CompleteEqualsZero()
        {
            // Arrange
            var create = new CreateBookDTO { Title = "T", Author = "A", Description = "D" };
            var book = new Book { Id = 0, Title = create.Title, Author = create.Author, Description = create.Description };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            _mapper.Map<Book>(create).Returns(book);

            _unitOfWork.CompleteAsync().Returns(0);

            var svc = CreateService();

            // Act
            var res = await svc.AddBookAsync(create);

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task DeleteBookAsync_ReturnsNotFound_When_BookMissing()
        {
            // Arrange
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Book?>(null));

            var svc = CreateService();

            // Act
            var res = await svc.DeleteBookAsync(5);

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteBookAsync_ReturnsSuccess_When_Deleted()
        {
            // Arrange
            var book = new Book { Id = 2, Title = "T", Author = "A", Description = "D" };
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(2).Returns(Task.FromResult<Book?>(book));

            // CompleteAsync called twice in service; return 1
            _unitOfWork.CompleteAsync().Returns(1);

            var svc = CreateService();

            // Act
            var res = await svc.DeleteBookAsync(2);

            // Assert
            Assert.True(res.Succeeded);
            Assert.True(res.Data);
            Assert.Equal("Book is deleted successfully", res.Message);
        }

        [Fact]
        public async Task DeleteBookAsync_ReturnsFail_When_CompleteIsZero()
        {
            // Arrange
            var book = new Book { Id = 3, Title = "T", Author = "A", Description = "D" };
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(3).Returns(Task.FromResult<Book?>(book));
            _unitOfWork.CompleteAsync().Returns(0);

            var svc = CreateService();

            // Act
            var res = await svc.DeleteBookAsync(3);

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task UpdateBookAsync_ReturnsNotFound_When_BookMissing()
        {
            // Arrange
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Book?>(null));

            var svc = CreateService();

            // Act
            var res = await svc.UpdateBookAsync(10, new UpdateBookDTO { Title = "X" });

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task UpdateBookAsync_ReturnsBadRequest_When_AllFieldsNull()
        {
            // Arrange
            var book = new Book { Id = 4, Title = "Old", Author = "A", Description = "D" };
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(4).Returns(Task.FromResult<Book?>(book));

            var svc = CreateService();

            // Act
            var res = await svc.UpdateBookAsync(4, new UpdateBookDTO { Title = null, Author = null, Description = null });

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task UpdateBookAsync_ReturnsSuccess_When_UpdateSucceeds()
        {
            // Arrange
            var book = new Book { Id = 5, Title = "Old", Author = "A", Description = "D" };
            var updatedDto = new UpdateBookDTO { Title = "New", Author = null, Description = null };
            var bookDto = new BookDTO { Id = 5, Title = "New", Author = "A", Description = "D", IsAvailable = true };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(5).Returns(Task.FromResult<Book?>(book));

            _unitOfWork.CompleteAsync().Returns(1);
            _mapper.Map<BookDTO>(Arg.Any<Book>()).Returns(bookDto);

            var svc = CreateService();

            // Act
            var res = await svc.UpdateBookAsync(5, updatedDto);

            // Assert
            Assert.True(res.Succeeded);
            Assert.NotNull(res.Data);
            Assert.Equal("New", res.Data!.Title);
        }

        [Fact]
        public async Task UpdateBookAsync_ReturnsFail_When_CompleteZero()
        {
            // Arrange
            var book = new Book { Id = 6, Title = "Old", Author = "A", Description = "D" };
            var updatedDto = new UpdateBookDTO { Title = "New" };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(6).Returns(Task.FromResult<Book?>(book));

            _unitOfWork.CompleteAsync().Returns(0);

            var svc = CreateService();

            // Act
            var res = await svc.UpdateBookAsync(6, updatedDto);

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public void GetAllBooksAsync_ReturnsOnlyAvailableBooks()
        {
            // Arrange
            var list = new List<Book>
            {
                new Book { Id = 1, Title = "A", Author = "a", Description = "d", IsAvailable = true },
                new Book { Id = 2, Title = "B", Author = "b", Description = "d", IsAvailable = false },
                new Book { Id = 3, Title = "C", Author = "c", Description = "d", IsAvailable = true },
            };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);

            // Return a plain IQueryable - EF Core's ToListAsync can execute against it in tests
            repo.GetAllAsQuerable().Returns(list.AsQueryable());

            // mapper should accept whatever source BookService passes (it passes the result of ToListAsync call)
            var mapped = list.Where(b => b.IsAvailable).Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                IsAvailable = b.IsAvailable
            }).ToList().AsEnumerable();

            // The service calls mapper.Map<IEnumerable<BookDTO>>(books) where books is the Task<List<Book>>
            // Accept any object and return the mapped set
            _mapper.Map<IEnumerable<BookDTO>>(Arg.Any<object>()).Returns(mapped);

            var svc = CreateService();

            // Act
            var res = svc.GetAllBooksAsync();

            // Assert
            Assert.True(res.Succeeded);
            Assert.NotNull(res.Data);
            var data = res.Data!.ToList();
            Assert.All(data, d => Assert.True(d.IsAvailable));
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsNotFound_When_Missing()
        {
            // Arrange
            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<Book?>(null));

            var svc = CreateService();

            // Act
            var res = await svc.GetBookByIdAsync(42);

            // Assert
            Assert.False(res.Succeeded);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetBookByIdAsync_ReturnsBook_When_Found()
        {
            // Arrange
            var book = new Book { Id = 7, Title = "T7", Author = "A7", Description = "D7" };
            var bookDto = new BookDTO { Id = 7, Title = "T7", Author = "A7", Description = "D7", IsAvailable = true };

            var repo = Substitute.For<IGenericRepository<Book, int>>();
            _unitOfWork.GetRepository<Book, int>().Returns(repo);
            repo.GetByIdAsync(7).Returns(Task.FromResult<Book?>(book));
            _mapper.Map<BookDTO>(book).Returns(bookDto);

            var svc = CreateService();

            // Act
            var res = await svc.GetBookByIdAsync(7);

            // Assert
            Assert.True(res.Succeeded);
            Assert.NotNull(res.Data);
            Assert.Equal(bookDto.Id, res.Data!.Id);
        }
    }
}
