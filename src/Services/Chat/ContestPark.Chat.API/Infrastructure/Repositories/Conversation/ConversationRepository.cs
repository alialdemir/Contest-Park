using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public class ConversationRepository : IConversationRepository
    {
        #region Private Variables

        private readonly IDocumentDbRepository<Documents.Conversation> _conversationRepository;
        private readonly ILogger<ConversationRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public ConversationRepository(IDocumentDbRepository<Documents.Conversation> conversationRepository,
                                      ILogger<ConversationRepository> logger)
        {
            _conversationRepository = conversationRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Konuşma güncelle
        /// </summary>
        /// <param name="conversation">Konuşma bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> UpdateAsync(Documents.Conversation conversation)
        {
            return _conversationRepository.UpdateAsync(conversation);
        }

        /// <summary>
        /// Eğer iki kullanıcı arasında konuşma varsa o conversation id'sini verir yoksa konuşma ekler ve eklediği conversation id döner
        /// </summary>
        /// <param name="senderUserId">Gönderen kullanıcı id</param>
        /// <param name="receiverUserId">Alıcı kullanıcı id</param>
        /// <returns>Conversation id</returns>
        public async Task<Documents.Conversation> AddOrGetConversationAsync(string senderUserId, string receiverUserId)
        {
            if (string.IsNullOrEmpty(senderUserId) || string.IsNullOrEmpty(receiverUserId))
                return null;

            Documents.Conversation conversation = GetConversationIdByParticipants(senderUserId, receiverUserId);
            if (conversation != null)
                return conversation;

            conversation = new Documents.Conversation
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId
            };

            bool isSuccess = await _conversationRepository.AddAsync(conversation);
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: iki kullanıcı arasında conversation ekleme işlemi başarısız oldu.", senderUserId, receiverUserId);

                return null;
            }

            return conversation;
        }

        /// <summary>
        /// İki kullanıcı arasındaki konuşma id verir
        /// </summary>
        /// <param name="senderUserId">Gönderen kullanıcı id</param>
        /// <param name="receiverUserId">Alıcı kullanıcı id</param>
        /// <returns>Conversation id</returns>
        private Documents.Conversation GetConversationIdByParticipants(string senderUserId, string receiverUserId)
        {
            string sql = @"SELECT TOP 1 * FROM c
                           WHERE c.SenderUserId = @senderUserId AND c.ReceiverUserId = @receiverUserId OR
                                 c.SenderUserId = @receiverUserId AND c.ReceiverUserId = @senderUserId";

            return _conversationRepository.QuerySingle<Documents.Conversation>(sql, new
            {
                senderUserId,
                receiverUserId
            });
        }

        #endregion Methods
    }
}