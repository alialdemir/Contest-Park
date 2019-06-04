using ContestPark.Core.CosmosDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ContestPark.Core.CosmosDb.Interfaces
{
    public interface IDocumentDbInitializer
    {
        IDocumentClient GetClient(DocumentDbConnection dbConnection, ConnectionPolicy connectionPolicy = null);
    }
}