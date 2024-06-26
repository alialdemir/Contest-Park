﻿using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public class MessageRepository : IMessageRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Message> _messageRepository;
        private readonly ILogger<MessageRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public MessageRepository(IRepository<Tables.Message> messageRepository,
                                 ILogger<MessageRepository> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Mesaj gönder
        /// </summary>
        /// <param name="message">Mesaj içeriği</param>
        /// <returns>İşlem durumu</returns>
        public long AddMessage(SendMessageModel message)
        {
            if (
                message == null ||
                string.IsNullOrEmpty(message.AuthorUserId) ||
                string.IsNullOrEmpty(message.ReceiverUserId) ||
                string.IsNullOrEmpty(message.Text)
                )
            {
                _logger.LogInformation("Mesaj eklenirken değerler boş geldi", message);

                return 0;
            }

            return _messageRepository.QuerySingleOrDefault<long>("SELECT FNC_AddMessage(@AuthorUserId, @ReceiverUserId, @AuthorUserId, @Text)", new
            {
                message.AuthorUserId,
                message.ReceiverUserId,
                message.Text
            });
        }

        /// <summary>
        /// Konuşmadaki tüm mesajları kaldırır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="conversationId">Konuşma id</param>
        /// <returns>İşlem sonucu başarılı ise true değilse false</returns>
        public async Task<bool> RemoveMessagesAsync(string userId, long conversationId)
        {
            bool isSuccess = await _messageRepository.ExecuteAsync("SP_RemoveMessages", new
            {
                userId,
                conversationId
            }, commandType: CommandType.StoredProcedure);

            if (!isSuccess)
            {
                _logger.LogCritical("CRITICAL: Mesajlar silinemedi. {userId} conversationId: {conversationId}", userId, conversationId);
            }

            return isSuccess;
        }

        /// <summary>
        /// Konuşma detayı
        /// </summary>
        /// <param name="conversationId">Konuşma id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Konuşma detayı</returns>
        public ServiceModel<ConversationDetailModel> ConversationDetail(string userId, string senderUserId, PagingModel paging)
        {
            string sql = @"SELECT
                           c.ConversationId,
                           m.CreatedDate as Date,
                           m.TEXT as Message,
                           m.AuthorUserId as SenderId,
                           m.AuthorUserId = @userId AS IsIncoming
                           FROM Messages m
                           INNER JOIN Conversations c ON c.ConversationId = m.ConversationId
                           WHERE m.ConversationId = c.ConversationId
                           AND ((c.ReceiverUserId = @userId AND c.SenderUserId = @senderUserId) OR (c.ReceiverUserId = @senderUserId AND c.SenderUserId = @userId))
                           AND ((c.ReceiverUserId = @userId AND m.ReceiverIsDeleted=FALSE) OR (c.SenderUserId= @userId AND m.SenderIsDeleted=FALSE))
                           ORDER BY m.CreatedDate ASC";

            return _messageRepository.ToServiceModel<ConversationDetailModel>(sql, new
            {
                userId,
                senderUserId
            }, pagingModel: paging);
        }

        /// <summary>
        /// Görülmemiş mesajları görüldü yapar
        /// </summary>
        /// <param name="userId"></param>
        public async void ChatSeen(string userId)
        {
            await _messageRepository.ExecuteAsync("SP_SeenAllChat", new
            {
                userId,
            }, commandType: CommandType.StoredProcedure);
        }

        #endregion Methods
    }
}
