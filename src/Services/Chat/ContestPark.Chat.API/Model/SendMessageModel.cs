namespace ContestPark.Chat.API.Model
{
    public class SendMessageModel
    {
        public string AuthorUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string Text { get; set; }
    }
}
