using Book_Lending_System.Core.Entities.Books;
using Book_Lending_System.Core.Entities.Identity;
using Book_Lending_System.Infrastructure.Persistence.Data.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Book_Lending_System.Infrastructure.Persistence.Data
{
    public class StoreDbContext:IdentityDbContext<ApplicationUser>
    {

        public StoreDbContext()
        {
            
        }
        public StoreDbContext(DbContextOptions<StoreDbContext>options):base(options) 
        {
        }

       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
    }
}
