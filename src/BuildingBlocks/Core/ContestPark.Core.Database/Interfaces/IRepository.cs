using ContestPark.Core.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.Database.Interfaces
{
    /// <summary>
    /// Document Db Repository arayüzü
    /// </summary>
    /// <typeparam name="T">Generic entity</typeparam>
    public interface IRepository<TEntity> where TEntity : class//, IEntity, new()
    {
        Task<int> CountAsync();

        TEntity FindById(dynamic id);

        IEnumerable<TEntity> FindByIds(IEnumerable<dynamic> ids);

        Task<bool> AddAsync(TEntity entity);

        Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> AddRangeAsync<TKey>(IEnumerable<TEntity> entities);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task<bool> RemoveAsync(dynamic id);

        IEnumerable<T> QueryMultiple<T>(string sql, object parameters = null, CommandType? commandType = null);

        T QuerySingleOrDefault<T>(string sql, object parameters = null, CommandType? commandType = null);

        IEnumerable<TThird> SpQuery<TFirst, TSecond, TThird>(string sql, Func<TFirst, TSecond, TThird> map, object parameters = null, string splitOn = "", CommandType? commandType = null);

        Task<bool> RemoveAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null);

        ServiceModel<TResult> ToServiceModel<TResult>(string sql, object parameters = null, CommandType? commandType = null, PagingModel pagingModel = null);
    }
}
