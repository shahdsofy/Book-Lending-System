using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Base;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Book_Lending_System.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly StoreDbContext dbContext;

        public GenericRepository(StoreDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public  IQueryable<TEntity> GetAllAsQuerable()
        {
            return  dbContext.Set<TEntity>();
        }
        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await dbContext.Set<TEntity>().FindAsync(id);
        }
        public async Task AddAsync(TEntity entity)
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            dbContext.Set<TEntity>().Remove(entity);
        }
        public void Update(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
        }

        public async Task<bool> CheckUserBorrowBookOrNot(string  userId)
        {
            var check =await dbContext.BorrowRecords.AnyAsync(r => r.UserId == userId && r.ReturnedAt == null);
            return check;
        }

        
    }
}
