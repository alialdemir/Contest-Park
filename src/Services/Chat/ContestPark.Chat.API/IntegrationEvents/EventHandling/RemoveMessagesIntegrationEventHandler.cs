using ContestPark.Chat.API.Infrastructure.Repositories.Message;
using ContestPark.Chat.API.IntegrationEvents.Events;
using ContestPark.Chat.API.Resources;
using ContestPark.EventBus.Abstractions;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.IntegrationEvents.EventHandling
{
    public class RemoveMessagesIntegrationEventHandler : IIntegrationEventHandler<RemoveMessagesIntegrationEvent>

    {
        #region Private Variables

        private readonly IMessageRepository _messageRepository;
        private readonly IEventBus _eventBus;

        #endregion Private Variables

        #region Constructor

        public RemoveMessagesIntegrationEventHandler(IMessageRepository messageRepository,
                                                    IEventBus eventBus)
        {
            _messageRepository = messageRepository;
            _eventBus = eventBus;
        }

        #endregion Constructor

        #region Handle

        /// <summary>
        /// Mesaj sil event handler
        /// </summary>
        public async Task Handle(RemoveMessagesIntegrationEvent @event)
        {
            bool isSuccess = await _messageRepository.RemoveMessages(@event.UserId, @event.ConversationId);
            if (!isSuccess)
            {
                var @eventSendError = new SendErrorMessageWithSignalrIntegrationEvent(@event.UserId,
                                                                                      ChatResource.AnErrorOccurredWhileDeletingYourMessagePleaseTryAgain);
                _eventBus.Publish(@eventSendError);
            }
        }

        #endregion Handle
    }
}