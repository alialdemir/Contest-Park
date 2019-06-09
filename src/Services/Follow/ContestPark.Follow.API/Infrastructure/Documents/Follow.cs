using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Follow.API.Infrastructure.Documents
{
    /// <summary>
    /// Bu dökümentin id değeri kullanıcı id
    /// </summary>
    public class Follow : DocumentBase
    {
        public string FollowUpUserId { get; set; }//Takip eden
        public string FollowedUserId { get; set; }//Takip edilen
    }
}