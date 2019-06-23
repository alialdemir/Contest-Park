using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Chat.API.Infrastructure.Documents
{
    public class Conversation : DocumentBase
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
    }
}