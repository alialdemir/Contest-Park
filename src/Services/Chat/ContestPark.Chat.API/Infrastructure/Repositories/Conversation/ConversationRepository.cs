using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public class ConversationRepository : IConversationRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Conversation> _conversationRepository;
        private readonly ILogger<ConversationRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public ConversationRepository(IRepository<Tables.Conversation> conversationRepository,
                                      ILogger<ConversationRepository> logger)
        {
            _conversationRepository = conversationRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Konuşma id'si ilgili kullanıcıya mı ait kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>Kullanıcı konuşmada varsa true yoksa false</returns>
        public bool IsConversationBelongUser(string userId, long conversationId)
        {
            string sql = @"SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM Conversations c WHERE c.ConversationId = @conversationId AND (c.SenderUserId = @userId OR c.ReceiverUserId = @userId))
                           THEN 1
                           ELSE 0
                           END)";

            return _conversationRepository.QuerySingleOrDefault<bool>(sql, new
            {
                userId,
                conversationId
            });
        }

        /// <summary>
        /// Okunmamış mesaj sayısını verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Okunmamış mesaj sayısı</returns>
        public int UnReadMessageCount(string userId)
        {
            /*
             SenderUnreadMessageCount ve ReceiverUnreadMessageCount alıcı ve gönderenin o konuşma içerisindeki okumadığı mesaj sayısını barındırır
             eğer okunmayan mesajların sayısının toplamını sayarak(count) genel olarak kaç mesaj okumadığını bulduk
             */
            string sql = @"SELECT
                           COUNT(c.ConversationId)
                           FROM Conversations c WHERE (c.SenderUserId = @userId AND c.SenderUnreadMessageCount > 0)
                           OR (c.ReceiverUserId = @userId AND c.ReceiverUnreadMessageCount > 0)";

            return _conversationRepository.QuerySingleOrDefault<int>(sql, new
            {
                userId
            });
        }

        /// <summary>
        /// Konuşmadaki okunma sayısını 0 okundu yapar
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="conversationId"></param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> AllMessagesRead(string userId, long conversationId)
        {
            bool isSuccess = await _conversationRepository.ExecuteAsync("SP_AllMessagesRead", new
            {
                userId,
                conversationId
            }, System.Data.CommandType.StoredProcedure);

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Okunmamış mesajlar okundu yapılamadı. {userId} conversationId: {conversationId}", userId, conversationId);
            }

            return isSuccess;
        }

        /// <summary>
        /// Kullanıcının ilgili conversation id'deki konuşma listesi
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Konuşma detayı</returns>
        public ServiceModel<MessageModel> UserMessages(string userId, PagingModel paging)
        {
            string sql = @"SELECT DISTINCT
                           c.ConversationId,
                           c.LastMessageDate AS DATE,
                           c.LastMessage AS Message,
                           CASE WHEN c.SenderUserId = @userId THEN c.ReceiverUserId ELSE c.SenderUserId END AS SenderUserId,
                           c.LastWriterUserId
                           FROM Conversations c
                           INNER JOIN Messages m ON m.ConversationId = c.ConversationId
                           WHERE (c.SenderUserId = @userId AND m.SenderIsDeleted = 0) OR (c.ReceiverUserId = @userId AND m.ReceiverIsDeleted = 0)
                           ORDER BY c.CreatedDate DESC";
            return _conversationRepository.ToServiceModel<MessageModel>(sql, new
            {
                userId,
            }, pagingModel: paging);
        }

        #endregion Methods
    }
}
