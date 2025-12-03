using Book_Lending_System.Core.Entities.Base;

namespace Book_Lending_System.Core.Contracts.Persistence
{
    public interface IUnitOfWork:IAsyncDisposable
    {
        IGenericRepository<TEntity,TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>;
        Task<int> CompleteAsync();
    }
}
