using Book_Lending_System.Core.Entities.Base;

namespace Book_Lending_System.Core.Entities.Books
{
    public class Book:BaseAuditableEntity<int>
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<BorrowRecord>? BorrowBooks { get; set; }

    }
}
