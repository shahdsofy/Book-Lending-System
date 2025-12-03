using AutoMapper;
using Book_Lending_System.Application.Abstraction;
using Book_Lending_System.Application.Mapping;
using Book_Lending_System.Application.Services.BorrowBook;
using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Book_Lending_System.Infrastructure.Persistence.Repositories;
using Book_Lending_System.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Threading.Tasks;
using Xunit;

public class BorrowBookServiceTests
{
    private StoreDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
            .Options;

        return new StoreDbContext(options);
    }

    private BorrowBookService CreateService(StoreDbContext dbContext, string userId)
    {
        var uow = new UnitOfWork(dbContext);
        var loggedInUser = Substitute.For<ILoggedInUserService>();
        loggedInUser.UserId.Returns(userId);

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        // إنشاء Configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }, loggerFactory);

        // إنشاء Mapper
        var mapper = config.CreateMapper();

        return new BorrowBookService(uow, loggedInUser, mapper);
    }

    // ---------------- BorrowBookAsync Tests ----------------

    [Fact]
    public async Task BorrowBookAsync_BookNotFound_ReturnsNotFound()
    {
        using var db = CreateInMemoryDb();
        var service = CreateService(db, "u1");

        var response = await service.BorrowBookAsync(1);

        Assert.False(response.Succeeded);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task BorrowBookAsync_BookNotAvailable_ReturnsBadRequest()
    {
        using var db = CreateInMemoryDb();
        db.Books.Add(new Book
        {
            Id = 1,
            Title = "T",
            Author = "A",
            Description = "D",
            IsAvailable = false,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.SaveChanges();

        var service = CreateService(db, "u1");

        var response = await service.BorrowBookAsync(1);

        Assert.False(response.Succeeded);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BorrowBookAsync_UserHasActiveBorrow_ReturnsBadRequest()
    {
        using var db = CreateInMemoryDb();
        db.Books.Add(new Book
        {
            Id = 1,
            Title = "T",
            Author = "A",
            Description = "D",
            IsAvailable = true,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.BorrowRecords.Add(new BorrowRecord
        {
            Id = 1,
            BookId = 2,
            UserId = "u1",
            BorrowedAt = System.DateTime.Now,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.SaveChanges();

        var service = CreateService(db, "u1");

        var response = await service.BorrowBookAsync(1);

        Assert.False(response.Succeeded);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task BorrowBookAsync_Success_ReturnsSuccess()
    {
        using var db = CreateInMemoryDb();
        db.Books.Add(new Book
        {
            Id = 1,
            Title = "T",
            Author = "A",
            Description = "D",
            IsAvailable = true,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.SaveChanges();

        var service = CreateService(db, "u2");

        var response = await service.BorrowBookAsync(1);

        Assert.True(response.Succeeded);
        Assert.Equal("Book borrowed successfully.", response.Data);

        var bookInDb = await db.Books.FindAsync(1);
        Assert.False(bookInDb!.IsAvailable);
    }

    // ---------------- ReturnBookAsync Tests ----------------

    [Fact]
    public async Task ReturnBookAsync_NoActiveBorrow_ReturnsNotFound()
    {
        using var db = CreateInMemoryDb();
        var service = CreateService(db, "u1");

        var response = await service.ReturnBookAsync(1);

        Assert.False(response.Succeeded);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReturnBookAsync_Success_ReturnsSuccess()
    {
        using var db = CreateInMemoryDb();
        db.Books.Add(new Book
        {
            Id = 2,
            Title = "T",
            Author = "A",
            Description = "D",
            IsAvailable = false,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.BorrowRecords.Add(new BorrowRecord
        {
            Id = 1,
            BookId = 2,
            UserId = "u1",
            BorrowedAt = System.DateTime.Now,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.SaveChanges();

        var service = CreateService(db, "u1");

        var response = await service.ReturnBookAsync(2);

        Assert.True(response.Succeeded);
        Assert.Equal("Book returned successfully.", response.Data);

        var bookInDb = await db.Books.FindAsync(2);
        Assert.True(bookInDb!.IsAvailable);
    }

    [Fact]
    public async Task ReturnBookAsync_CompleteZero_ReturnsFail()
    {
        using var db = CreateInMemoryDb();
        db.Books.Add(new Book
        {
            Id = 3,
            Title = "T",
            Author = "A",
            Description = "D",
            IsAvailable = false,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.BorrowRecords.Add(new BorrowRecord
        {
            Id = 1,
            BookId = 3,
            UserId = "u1",
            BorrowedAt = System.DateTime.Now,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        db.SaveChanges();

        // UnitOfWork وهمي لإرجاع CompleteAsync = 0
        var loggedInUser = Substitute.For<ILoggedInUserService>();
        loggedInUser.UserId.Returns("u1");

        var uow = Substitute.For<IUnitOfWork>();
        var bookRepo = new GenericRepository<Book, int>(db);
        var borrowRepo = new GenericRepository<BorrowRecord, int>(db);
        uow.GetRepository<Book, int>().Returns(bookRepo);
        uow.GetRepository<BorrowRecord, int>().Returns(borrowRepo);
        uow.CompleteAsync().Returns(0);

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        // إنشاء Configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }, loggerFactory);

        // إنشاء Mapper
        var mapper = config.CreateMapper();

        var service = new BorrowBookService(uow, loggedInUser, mapper);

        var response = await service.ReturnBookAsync(3);

        Assert.False(response.Succeeded);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
