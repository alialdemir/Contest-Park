using ContestPark.Chat.API.Model;
using ContestPark.Core.CosmosDb.Extensions;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public class ConversationRepository : IConversationRepository
    {
        #region Private Variables

        private readonly IRepository<Documents.Conversation> _conversationRepository;
        private readonly ILogger<ConversationRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public ConversationRepository(IRepository<Documents.Conversation> conversationRepository,
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

            return _conversationRepository.QuerySingleOrDefault<Documents.Conversation>(sql, new
            {
                senderUserId,
                receiverUserId
            });
        }

        /// <summary>
        /// Conversation id'deki mesajın göndereni user id mi kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>Gönderen user id ise true değilse false</returns>
        public bool IsSender(string userId, string conversationId)
        {
            string sql = @"SELECT TOP 1 VALUE CONTAINS(c.SenderUserId, @userId) FROM c
                           WHERE c.id=@conversationId";

            return _conversationRepository.QuerySingleOrDefault<bool>(sql, new
            {
                userId,
                conversationId
            });
        }

        /// <summary>
        /// Konuşma id'si ilgili kullanıcıya mı ait kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>Kullanıcı konuşmada varsa true yoksa false</returns>
        public bool IsConversationBelongUser(string userId, string conversationId)
        {
            string sql = @"SELECT TOP 1 VALUE (c.SenderUserId=@userId OR c.ReceiverUserId=@userId) FROM c
                           WHERE c.id=@conversationId";

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
            string sql = @"SELECT VALUE
                           COUNT(c.id)
                           FROM c WHERE (c.SenderUserId=@userId AND c.SenderUnreadMessageCount > 0)
                           OR (c.ReceiverUserId=@userId AND c.ReceiverUnreadMessageCount > 0)";

            return _conversationRepository.QuerySingleOrDefault<int>(sql, new
            {
                userId
            });
        }

        /// <summary>
        /// Konuşmadaki okunma sayısını 0 okundu yapar
        /// </summary>
        /// <param name="useerId"></param>
        /// <param name="conversationId"></param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> AllMessagesRead(string useerId, string conversationId)
        {
            Documents.Conversation conversation = _conversationRepository.FindById(conversationId);
            if (conversation == null || !(conversation.SenderUserId == useerId || conversation.ReceiverUserId == useerId))// konuşma  o kullanıcıya mı ait kontrol edildi
                return false;

            if (conversation.SenderUserId == useerId)
            {
                conversation.SenderUnreadMessageCount = 0;
            }
            else if (conversation.ReceiverUserId == useerId)
            {
                conversation.ReceiverUnreadMessageCount = 0;
            }

            return await _conversationRepository.UpdateAsync(conversation);
        }

        /// <summary>
        /// Kullanıcının ilgili conversation id'deki konuşma listesi
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Konuşma detayı</returns>
        public ServiceModel<MessageModel> UserMessages(string userId, PagingModel paging)
        {
            string sql = @"SELECT VALUE
                           {
                             Date: c.LastMessageDate ,
                             Message:  c.LastMessage,
                             ConversationId: c.id,
                             SenderUserId: c.SenderUserId=@userId ? c.ReceiverUserId: c.SenderUserId,
                             LastWriterUserId: c.LastWriterUserId
                            }
                            FROM c
                            WHERE c.SenderUserId=@userId OR c.ReceiverUserId=@userId
                            ORDER BY c.CreatedDate DESC";
            return _conversationRepository.ToServiceModel<Documents.Conversation, MessageModel>(sql, new
            {
                userId,
            }, paging);
        }

        #endregion Methods
    }
}