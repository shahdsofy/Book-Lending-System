using AutoMapper;
using Book_Lending_System.Application.Abstraction;
using Book_Lending_System.Application.Mapping;
using Book_Lending_System.Application.Services.BorrowBook;
using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

public class BorrowBookServiceTests
{

    private BorrowBookService CreateService(StoreDbContext context, string userId="1" )
    {
        //unit of work
        var unitOfWork=new UnitOfWork(context);

        //logged in user service (NSubstitute)
        var loggedInUser = Substitute.For<ILoggedInUserService>();
        loggedInUser.UserId.Returns(userId);


        //mapper
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var mapper=new MapperConfiguration(cfg=>
        {
            cfg.AddProfile<MappingProfile>();
        },loggerFactory).CreateMapper();

        return new BorrowBookService(unitOfWork, loggedInUser, mapper);
    }


    private StoreDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(databaseName:  Guid.NewGuid().ToString())
            .Options;

        return new StoreDbContext(options);
    }

    [Fact]
    public async Task BorrowBookAsync_ShouldBorrowBook_WhenBookIsAvailable()
    {
        // Arrange
        using var context = CreateInMemoryDb();

        context.Books.Add(new Book
        {
            Id = 1,
            Title = "Clean Code",
            IsAvailable = true,
            Author = "Robert C. Martin",
            Description = "A Handbook of Agile Software Craftsmanship",
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        // Act
        var result = await service.BorrowBookAsync(1);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Success", result.Message);

        var book = await context.Books.FindAsync(1);
        Assert.False(book.IsAvailable);

        var borrowRecord = context.BorrowRecords.FirstOrDefault();
        Assert.NotNull(borrowRecord);
        Assert.Equal("1", borrowRecord.UserId);
    }

    [Fact]
    public async Task BorrowBookAsync_ShouldFail_WhenBookNotFound()
    {
        using var context = CreateInMemoryDb();
        var service = CreateService(context);

        var result = await service.BorrowBookAsync(99);

        Assert.False(result.Succeeded);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }


    [Fact]
    public async Task BorrowBookAsync_ShouldFail_WhenUserHasActiveBorrow()
    {
        using var context = CreateInMemoryDb();
        context.Books.Add(new Book
        {
            Id = 2,
            Title = "The Pragmatic Programmer",
            IsAvailable = true,
            Author = "Andrew Hunt and David Thomas",
            Description = "Your Journey to Mastery",
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });

        context.BorrowRecords.Add(new BorrowRecord
        {
            Id = 1,
            BookId = 2,
            UserId = "1",
            BorrowedAt = DateTime.Now,
            ReturnedAt = null,
            CreatedBy = "TestUser",
            LastModifiedBy = "TestUser"
        });
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result= await service.BorrowBookAsync(2);

        Assert.False(result.Succeeded);
        Assert.Contains("only borrow one book", result.Message);
    }



    [Fact]
    public async Task ReturnBookAsync_ShouldReturnBookSuccessfully()
    {
        using var context = CreateInMemoryDb();

        context.Books.Add(new Book
        {
            Id = 1,
            IsAvailable = false,
            Title = "Refactoring",
            Author = "Martin Fowler",
            Description = "Improving the Design of Existing Code",
            CreatedBy= "TestUser",
            LastModifiedBy= "TestUser"
        });

        context.BorrowRecords.Add(new BorrowRecord
        {
            Id= 1,
            BookId = 1,
            UserId = "1",
            BorrowedAt = DateTime.Now,
            CreatedBy= "TestUser",
            LastModifiedBy= "TestUser"
        });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.ReturnBookAsync(1);

        Assert.True(result.Succeeded);

        var book = await context.Books.FindAsync(1);
        Assert.True(book.IsAvailable);

        var record = context.BorrowRecords.First();
        Assert.NotNull(record.ReturnedAt);
    }


}
