namespace Book_Lending_System.Application.Abstraction.Services.BorrowBook
{
    public interface IOverdueBooksService
    {
        Task ProcessOverdueBooksAsync();
    }
}
