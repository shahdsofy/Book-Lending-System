using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Infrastructure.Persistence.Data.Config.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Book_Lending_System.Infrastructure.Persistence.Data.Config.Books
{
    internal class ClassConfigurations : BaseAuditableEntityConfigurations<Book, int>
    {
        public override void Configure(EntityTypeBuilder<Book> builder)
        {
            base.Configure(builder);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(b=>b.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

        }
    }
}
