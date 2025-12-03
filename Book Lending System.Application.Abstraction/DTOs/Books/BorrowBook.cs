namespace Book_Lending_System.Application.Abstraction.DTOs.Books
{
    public class BorrowBook
    {
        public string UserId { get; set; }
        public int BookId { get; set; }
        public DateTime Borrowedat  { get; set; }
    }
}
