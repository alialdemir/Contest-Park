namespace ContestPark.Core.CosmosDb.Models
{
    public class DocumentDbConnection
    {
        public string CosmosDbServiceEndpoint { get; set; }
        public string CosmosDbAuthKeyOrResourceToken { get; set; }
        public string CosmosDbDatabaseId { get; set; }
    }
}