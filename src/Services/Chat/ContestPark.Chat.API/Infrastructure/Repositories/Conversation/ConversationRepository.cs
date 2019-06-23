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
        /// İki kullanıcı arası mesaj ekle
        /// </summary>
        /// <param name="senderUserId">Gönderen kullanıcı id</param>
        /// <param name="receiverUserId">Alıcı kullanıcı id</param>
        /// <returns>Conversation id</returns>
        public async Task<string> AddConversationAsync(string senderUserId, string receiverUserId)
        {
            if (string.IsNullOrEmpty(senderUserId) || string.IsNullOrEmpty(receiverUserId))
                return string.Empty;

            var conversation = new Documents.Conversation
            {
                SenderUserId = senderUserId,
                ReceiverUserId = receiverUserId
            };

            bool isSuccess = await _conversationRepository.AddAsync(conversation);
            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: iki kullanıcı arasında conversation ekleme işlemi başarısız oldu.", senderUserId, receiverUserId);

                return string.Empty;
            }

            return conversation.Id;
        }

        /// <summary>
        /// İki kullanıcı arasındaki konuşma id verir
        /// </summary>
        /// <param name="senderUserId">Gönderen kullanıcı id</param>
        /// <param name="receiverUserId">Alıcı kullanıcı id</param>
        /// <returns>Conversation id</returns>
        public string GetConversationIdByParticipants(string senderUserId, string receiverUserId)
        {
            string sql = @"SELECT TOP 1 c.id FROM c
                           WHERE c.SenderUserId = @senderUserId AND c.ReceiverUserId = @receiverUserId";

            return _conversationRepository.QuerySingle<string>(sql, new
            {
                senderUserId,
                receiverUserId
            });
        }

        #endregion Methods
    }
}