using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public class Like : DocumentBase
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
