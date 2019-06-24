using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Chat.API.Infrastructure.Documents
{
    public class Block : DocumentBase
    {
        public string SkirterUserId { get; set; }
        public string DeterredUserId { get; set; }
    }
}