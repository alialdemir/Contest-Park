using ContestPark.EventBus.Events;

namespace ContestPark.Chat.API.IntegrationEvents.Events
{
    public class SendMessageWithSignalrIntegrationEvent : IntegrationEvent
    {
        public string AuthorUserId { get; set; }
        public string ConversationId { get; set; }
        public string Text { get; set; }

        public SendMessageWithSignalrIntegrationEvent(string authorUserId, string conversationId, string text)
        {
            AuthorUserId = authorUserId;
            ConversationId = conversationId;
            Text = text;
        }
    }
}