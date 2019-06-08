using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Core.CosmosDb.Interfaces
{
    /// <summary>
    /// Document Db Repository arayüzü
    /// </summary>
    /// <typeparam name="T">Generic entity</typeparam>
    public interface IDocumentDbRepository<TDocument> where TDocument : class, IDocument, new()
    {
        Task Init();

        IDocumentClient Client { get; }
        Uri CollectionUri { get; }

        Task<int> CountAsync();

        TDocument GetById(string id);

        IEnumerable<TDocument> GetByIds(params string[] ids);

        Task<bool> InsertAsync(TDocument document);

        Task<bool> InsertAsync(IEnumerable<TDocument> documents);

        Task<bool> UpdateAsync(TDocument document);

        Task<bool> DeleteAsync(string id);

        IQueryable<T> Query<T>(string sqlExpression, FeedOptions feedOptions = null);

        IQueryable<T> Query<T>(SqlQuerySpec querySpec, FeedOptions feedOptions = null);
    }
}