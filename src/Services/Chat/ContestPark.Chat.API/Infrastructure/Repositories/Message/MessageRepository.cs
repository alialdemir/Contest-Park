using ContestPark.Chat.API.Infrastructure.Repositories.Conversation;
using ContestPark.Chat.API.Model;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        #region Private Variables

        private readonly IRepository<Documents.Message> _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<MessageRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public MessageRepository(IRepository<Documents.Message> messageRepository,
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
        public Task<bool> AddMessage(SendMessageModel message)
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

        /// <summary>
        /// Okunmamış mesajları okundu yapar
        /// </summary>
        /// <param name="useerId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AllMessagesRead(string useerId, string conversationId)
        {
            string sql = @"SELECT * FROM c WHERE c.ConversationId=@conversationId AND c.AuthorUserId!=@useerId";

            List<Documents.Message> messages = _messageRepository.QueryMultiple<Documents.Message>(sql, new
            {
                useerId,
                conversationId
            }).ToList();

            messages.ForEach(message => message.ReceiverIsReadMessage = true);

            return await _messageRepository.UpdateRangeAsync(messages);
        }

        /// <summary>
        /// Konuşma detayı
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Konuşma detayı</returns>
        public ServiceModel<ConversationDetailModel> ConversationDetail(string conversationId, PagingModel paging)
        {
            string sql = @"SELECT
                           c.CreatedDate as Date,
                           c.Text as Message,
                           c.AuthorUserId as SenderId
                           FROM c WHERE c.ConversationId=@conversationId";

            return _messageRepository.ToServiceModel<Documents.Message, ConversationDetailModel>(sql, new
            {
                conversationId
            }, paging);
        }

        #endregion Methods
    }
}