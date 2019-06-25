using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Model;
using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Message> _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<MessageRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public MessageRepository(IDocumentDbRepository<Documents.Message> messageRepository,
            IConversationRepository conversationRepository,
                                 ILogger<MessageRepository> logger)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _logger = logger;
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

        /// <summary>
        /// Konuşmadaki tüm mesajları kaldırır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>İşlem sonucu başarılı ise true değilse false</returns>
        public async Task<bool> RemoveMessages(string userId, string conversationId)
        {
            bool isSender = _conversationRepository.IsSender(userId, conversationId);

            /*
              eğer parametreden gelen user id konuşmayı başlatan ise SenderIsDeleted false olanları getirdik
              eğer parametreden gelen user id mesajı alan taraf ise ReceiverIsDeleted false olanları getirdik
             */
            string sql = @"SELECT * FROM c
                           WHERE ((@isSender=true AND c.SenderIsDeleted=false) OR (@isSender=false AND c.ReceiverIsDeleted=false))
                           AND c.ConversationId=@conversationId";

            IEnumerable<Documents.Message> messages = _messageRepository.QueryMultiple<Documents.Message>(sql, new
            {
                isSender,
                conversationId
            });

            if (messages.Count() == 0)// tüm mesajlar kaldırılmış ise tekrar update etmesin diye ekledim
                return true;

            messages.ToList().ForEach(message =>
            {
                if (isSender)// Mesajı kaldırma isteği yollayan kişi gönderen ise gönderenin mesajlarını silindi yapıyoruz
                {
                    message.SenderIsDeleted = true;
                }
                else// Mesajı kaldırma isteği yollayan kişi alıcı ise alıcının mesajlarını siliyoruz
                {
                    message.ReceiverIsDeleted = true;
                }
            });

            bool isSuccess = await _messageRepository.UpdateRangeAsync(messages);
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Mesajlar silinemedi. {userId} conversationId: {conversationId}", userId, conversationId);
            }

            return isSuccess;
        }

        #endregion Methods
    }
}