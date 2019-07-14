using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Resources;
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
        private readonly IEventBus _eventBus;
        private readonly ILogger<SendMessageIntegrationEventHandler> _logger;

        #endregion Private Variables

        #region Constructor

        public SendMessageIntegrationEventHandler(IConversationRepository conversationRepository,
                                                  IMessageRepository messageRepository,
                                                  IEventBus eventBus,
                                                  ILogger<SendMessageIntegrationEventHandler> logger)
        {
            _conversationRepository = conversationRepository;
            _messageRepository = messageRepository;
            _eventBus = eventBus;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Mesajma işlemini handler eder
        /// </summary>
        /// <param name="event">Mesaj bilgisi</param>
        public Task Handle(SendMessageIntegrationEvent @event)
        {
            long conversationId = _messageRepository.AddMessage(new Model.SendMessageModel
            {
                Text = @event.Message,
                ReceiverUserId = @event.ReceiverUserId,
                AuthorUserId = @event.SenderUserId
            });
            if (conversationId <= 0)
            {
                _logger.LogCritical("Mesaj eklenirken hata oluştu.",
                                    @event.SenderUserId,
                                    @event.ReceiverUserId);

                SendErrorMessage(@event.SenderUserId);

                return Task.CompletedTask;
            }

            var @SendMessageEvent = new SendMessageWithSignalrIntegrationEvent(@event.SenderUserId,
                                                                               conversationId,
                                                                               @event.Message);
            _eventBus.Publish(@SendMessageEvent);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Kullanıcı id'ye mesaj gönderilemedi error mesajı gönderir
        /// </summary>
        /// <param name="userId">Error mesaj gösterilecek kullanıcı id</param>
        private void SendErrorMessage(string userId)
        {
            var @event = new SendErrorMessageWithSignalrIntegrationEvent(userId,
                                                                         ChatResource.MessageSendingFailedPleaseTryAgain);
            _eventBus.Publish(@event);
        }

        #endregion Methods
    }
}
