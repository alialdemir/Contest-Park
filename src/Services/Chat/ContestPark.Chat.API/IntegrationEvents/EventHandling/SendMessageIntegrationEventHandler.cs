using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Resources;
using ContestPark.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
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
        public async Task Handle(SendMessageIntegrationEvent @event)
        {
            var conversation = await _conversationRepository.AddOrGetConversationAsync(@event.SenderUserId, @event.ReceiverUserId);
            if (conversation == null)
            {
                _logger.LogCritical("Mesajlaşma sırasında iki kullanıcı arasındaki conversation id alınırken hata oluştu.",
                                    @event.SenderUserId,
                                    @event.ReceiverUserId);

                SendErrorMessage(@event.SenderUserId);

                return;
            }

            bool isSuccess = await _messageRepository.AddMessage(new Model.SendMessageModel
            {
                Text = @event.Message,
                ConversationId = conversation.Id,
                AuthorUserId = @event.SenderUserId
            });
            if (!isSuccess)
            {
                _logger.LogCritical("Mesaj eklenirken hata oluştu.",
                                    conversation,
                                    @event.SenderUserId,
                                    @event.ReceiverUserId);

                SendErrorMessage(@event.SenderUserId);

                return;
            }

            if (@event.SenderUserId == conversation.SenderUserId)// Okumadığı mesaj sayısı güncellendi
            {
                conversation.SenderUnreadMessageCount += 1;
            }
            else if (@event.ReceiverUserId == conversation.ReceiverUserId)
            {
                conversation.ReceiverUnreadMessageCount += 1;
            }

            conversation.LastMessage = @event.Message;
            conversation.LastWriterUserId = @event.SenderUserId;
            conversation.LastMessageDate = DateTime.Now;
            isSuccess = await _conversationRepository.UpdateAsync(conversation);
            if (!isSuccess)
            {
                _logger.LogInformation("Mesaj gönderilme işlemi sırasında son mesaj güncelleme işleminde hata alındı.", conversation.Id);
            }

            var @SendMessageEvent = new SendMessageWithSignalrIntegrationEvent(@event.SenderUserId,
                                                                               conversation.Id,
                                                                               @event.Message);
            _eventBus.Publish(@SendMessageEvent);
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