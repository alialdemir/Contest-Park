using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.IntegrationEvents.EventHandling
{
    public class SendMessageIntegrationEventHandler : IIntegrationEventHandler<SendMessageIntegrationEvent>
    {
        #region Private Variables

        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<SendMessageIntegrationEventHandler> _logger;

        #endregion Private Variables

        #region Constructor

        public SendMessageIntegrationEventHandler(IConversationRepository conversationRepository,
                                                IMessageRepository messageRepository,
                                                ILogger<SendMessageIntegrationEventHandler> logger)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        public async Task Handle(SendMessageIntegrationEvent @event)
        {
            string conversationId = await GetConversationId(@event.SenderUserId, @event.ReceiverUserId);
            if (string.IsNullOrEmpty(conversationId))
            {
                _logger.LogCritical("Mesajlaşma sırasında iki kullanıcı arasındaki conversation id alınırken hata oluştu.",
                                    conversationId,
                                    @event.SenderUserId,
                                    @event.ReceiverUserId);
                // TODO: signalr ile mesaj gönderilemedi mesajı iletilmeli

                return;
            }

            bool isSuccess = await _messageRepository.AddMessage(new Model.MessageModel
            {
                Text = @event.Message,
                ConversationId = conversationId,
                AuthorUserId = @event.AuthorUserId
            });

            if (!isSuccess)
            {
                _logger.LogCritical("Mesaj eklenirken hata oluştu.",
                                    conversationId,
                                    @event.SenderUserId,
                                    @event.ReceiverUserId);

                // TODO: signalr ile mesaj gönderilemedi mesajı iletilmeli
            }

            // TODO: konuştuğu kişi online ise signalr ile send mesaj
        }

        /// <summary>
        /// İki kullanıcı arasınadki conversation id verir eğer yoksa ekler
        /// </summary>
        /// <param name="senderUserId">Gönderen kullanıcı id</param>
        /// <param name="receiverUserId">Alıcı kullanıcı id</param>
        /// <returns>Conversation id</returns>
        private async Task<string> GetConversationId(string senderUserId, string receiverUserId)
        {
            string conversationId = _conversationRepository.GetConversationIdByParticipants(senderUserId, receiverUserId);
            if (string.IsNullOrEmpty(conversationId))
                conversationId = await _conversationRepository.AddConversationAsync(senderUserId, receiverUserId);

            return conversationId;
        }

        #endregion Methods
    }
}