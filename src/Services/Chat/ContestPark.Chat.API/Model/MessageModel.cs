namespace ContestPark.Chat.API.Model
{
    public class MessageModel
    {
        public string AuthorUserId { get; set; }
        public string ConversationId { get; set; }
        public string Text { get; set; }
    }
}