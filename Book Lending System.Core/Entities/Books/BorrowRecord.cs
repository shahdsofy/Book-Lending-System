using Book_Lending_System.Core.Entities.Base;
using Book_Lending_System.Core.Entities.Identity;

namespace Book_Lending_System.Core.Entities.Books
{
    public class BorrowRecord:BaseAuditableEntity<int>
    {
        public int BookId { get; set; }
        public Book Book { get; set; } 

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime BorrowedAt { get; set; }

        public DateTime? ReturnedAt { get; set; }

    }
}
