namespace ContestPark.Chat.API.Model
{
    public class SendMessageModel
    {
        public string AuthorUserId { get; set; }
        public string ConversationId { get; set; }
        public string Text { get; set; }
    }
}