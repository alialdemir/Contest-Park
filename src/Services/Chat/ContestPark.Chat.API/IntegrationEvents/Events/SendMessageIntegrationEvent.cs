using ContestPark.EventBus.Events;

namespace ContestPark.Chat.API.IntegrationEvents.Events
{
    public class SendMessageIntegrationEvent : IntegrationEvent
    {
        public SendMessageIntegrationEvent(string senderUserId, string receiverUserId, string message)
        {
            SenderUserId = senderUserId;
            ReceiverUserId = receiverUserId;
            Message = message;
        }

        public string SenderUserId { get; }
        public string ReceiverUserId { get; }
        public string Message { get; }
    }
}