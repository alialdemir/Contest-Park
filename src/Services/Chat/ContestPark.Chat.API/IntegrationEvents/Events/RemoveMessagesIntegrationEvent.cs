using ContestPark.EventBus.Events;

namespace ContestPark.Chat.API.IntegrationEvents.Events
{
    public class RemoveMessagesIntegrationEvent : IntegrationEvent
    {
        public RemoveMessagesIntegrationEvent(string userId, string conversationId)
        {
            UserId = userId;
            ConversationId = conversationId;
        }

        public string UserId { get; }
        public string ConversationId { get; }
    }
}