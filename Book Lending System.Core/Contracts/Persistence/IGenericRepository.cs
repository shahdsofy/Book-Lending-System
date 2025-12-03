using Book_Lending_System.Core.Entities.Base;

namespace Book_Lending_System.Core.Contracts.Persistence
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> GetAllAsQuerable();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> CheckUserBorrowBookOrNot(string userId);
    }
}
