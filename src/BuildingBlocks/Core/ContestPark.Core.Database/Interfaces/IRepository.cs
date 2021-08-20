using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.Database.Interfaces
{
    /// <summary>
    /// Document Db Repository arayüzü
    /// </summary>
    /// <typeparam name="T">Generic entity</typeparam>
    public interface IRepository<TEntity> : IQueryRepository where TEntity : class//, IEntity, new()
    {
        Task<int> CountAsync();

        TEntity FindById(dynamic id);

        IEnumerable<TEntity> FindByIds(IEnumerable<dynamic> ids);

        Task<int?> AddAsync(TEntity entity);

        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> AddRangeAsync<TKey>(IEnumerable<TEntity> entities);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> RemoveAsync(dynamic id);

        Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int?> AddAndGetIdAsync(TEntity entity);

        Task<Key> AddAsync<Key>(TEntity entity);
    }
}
