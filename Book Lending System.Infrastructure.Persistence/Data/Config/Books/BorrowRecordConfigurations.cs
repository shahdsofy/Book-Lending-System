using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence.Data.Config.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Book_Lending_System.Infrastructure.Persistence.Data.Config.Books
{
    internal class BorrowRecordConfigurations:BaseAuditableEntityConfigurations<BorrowRecord,int>
    {
        public override void Configure(EntityTypeBuilder<BorrowRecord> builder)
        {
            base.Configure(builder);

            builder.Property(b => b.BorrowedAt)
               .IsRequired();

            builder.Property(b => b.ReturnedAt)
             .IsRequired(false);

            // Relationship: BorrowBook * --- 1 User
            builder.HasOne(b => b.User)
                   .WithMany()
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship: BorrowBook * --- 1 Book
            builder.HasOne(b => b.Book)
                   .WithMany(bk => bk.BorrowBooks)
                   .HasForeignKey(b => b.BookId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
