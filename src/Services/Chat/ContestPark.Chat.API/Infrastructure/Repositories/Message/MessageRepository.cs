using ContestPark.Chat.API.Model;
using ContestPark.Core.CosmosDb.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Message> _messageRepository;

        #endregion Private Variables

        #region Constructor

        public MessageRepository(IDocumentDbRepository<Documents.Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        /// <param name="message">Mesaj içeriği</param>
        /// <returns>İşlem durumu</returns>
        public Task<bool> AddMessage(MessageModel message)
        {
            if (
                message == null ||
                string.IsNullOrEmpty(message.AuthorUserId) ||
                string.IsNullOrEmpty(message.ConversationId) ||
                string.IsNullOrEmpty(message.Text)
                )
                return Task.FromResult(false);

            return _messageRepository.AddAsync(new Documents.Message
            {
                AuthorUserId = message.AuthorUserId,
                ConversationId = message.ConversationId,
                Text = message.Text
            });
        }

        #endregion Methods
    }
}