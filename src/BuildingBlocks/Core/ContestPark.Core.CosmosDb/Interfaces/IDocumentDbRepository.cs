using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
        TDocument GetById(string id);

        Task<bool> InsertAsync(TDocument document);

        Task<bool> InsertAsync(IEnumerable<TDocument> documents);

        Task<bool> UpdateAsync(TDocument document);

        Task<bool> DeleteAsync(string id);

        IQueryable<T> Query<T>(string sqlExpression, FeedOptions feedOptions = null);

        IQueryable<T> Query<T>(SqlQuerySpec querySpec, FeedOptions feedOptions = null);
    }
}