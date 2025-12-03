using Book_Lending_System.Core.Contracts.Persistence;
using Book_Lending_System.Core.Entities.Base;
using Book_Lending_System.Infrastructure.Persistence.Data;
using Book_Lending_System.Infrastructure.Persistence.Repositories;
using System.Collections.Concurrent;

namespace Book_Lending_System.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ConcurrentDictionary<string, object> repositories;
        private readonly StoreDbContext dbContext;

        public UnitOfWork(StoreDbContext dbContext)
        {
            repositories = new ConcurrentDictionary<string, object>();
            this.dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>
        {
          return  (IGenericRepository<TEntity, TKey>)repositories.GetOrAdd(typeof(TEntity).Name, new GenericRepository<TEntity, TKey>(dbContext));
        }


        public async ValueTask DisposeAsync()
        {
           await dbContext.DisposeAsync();
        }
        public async Task<int> CompleteAsync()
        {
           return await dbContext.SaveChangesAsync();
        }
    }
}
