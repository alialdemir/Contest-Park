using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Chat.API.Infrastructure.Documents
{
    public class Message : DocumentBase
    {
        public string ConversationId { get; set; }

        public string Text { get; set; }

        public string AuthorUserId { get; set; }

        public bool VisibilityStatus { get; set; } = true;

        public bool ReceiverDeletingStatus { get; set; } = true;

        public bool SenderDeletingStatus { get; set; } = true;

        public bool IsDeleted { get; set; }
    }
}