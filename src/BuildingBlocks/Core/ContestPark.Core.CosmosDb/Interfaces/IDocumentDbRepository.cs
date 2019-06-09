using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContestPark.Core.CosmosDb.Interfaces
{
    /// <summary>
    /// Document Db Repository arayüzü
    /// </summary>
    /// <typeparam name="T">Generic entity</typeparam>
    public interface IDocumentDbRepository<TDocument> where TDocument : class, IDocument, new()
    {
        Task<int> CountAsync();

        TDocument FindById(string id);

        IEnumerable<TDocument> FindByIds(IEnumerable<string> ids);

        Task<bool> AddAsync(TDocument document);

        Task<bool> AddRangeAsync(IEnumerable<TDocument> documents);

        Task<bool> UpdateAsync(TDocument document);

        Task<bool> UpdateRangeAsync(IEnumerable<TDocument> entities);

        Task<bool> RemoveAsync(string id);

        IEnumerable<T> QueryMultipleAsync<T>(string sql, object parameters = null);

        TDocument QuerySingleAsync(string sql, object parameters = null);

        T QuerySingleAsync<T>(string sql, object parameters = null);

        Task<bool> RemoveAsync(Expression<Func<TDocument, bool>> predicate);
    }
}