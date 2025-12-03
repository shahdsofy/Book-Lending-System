using Book_Lending_System.Application.Abstraction;
using Book_Lending_System.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Book_Lending_System.Infrastructure.Persistence.Data.Interceptors
{
    public class CustomSaveChangesInterceptors:SaveChangesInterceptor
    {
        private readonly ILoggedInUserService loggedInUserService;

        public CustomSaveChangesInterceptors(ILoggedInUserService loggedInUserService)
        {
            this.loggedInUserService = loggedInUserService;
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context!);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context!);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
       

        private void UpdateEntities(DbContext dbContext)
        {
            foreach(var entry in dbContext.ChangeTracker.Entries<BaseAuditableEntity<int>>().Where(e=>e.State is EntityState.Added or EntityState.Modified))
            {
        
                if(entry.State==EntityState.Added)
                {
                    entry.Entity.CreatedOn=DateTime.Now;
                    entry.Entity.CreatedBy = loggedInUserService.UserId;
                }
                entry.Entity.LastModifiedOn=DateTime.Now;
                entry.Entity.LastModifiedBy = loggedInUserService.UserId;
            }
        }
    }
}
