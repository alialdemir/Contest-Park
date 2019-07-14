using ContestPark.Core.Database.Models;
using Dapper;
using System;

namespace ContestPark.Chat.API.Infrastructure.Tables
{
    [Table("Conversations")]
    public class Conversation : EntityBase
    {
        [Key]
        public long ConversationId { get; set; }

        public string SenderUserId { get; set; }

        public string ReceiverUserId { get; set; }

        public short SenderUnreadMessageCount { get; set; }// bu konuşmadaki gönderenin okumadığı mesaj sayısı

        public short ReceiverUnreadMessageCount { get; set; }// bu konuşmadaki alıcının okumadığı  mesaj sayısı

        public string LastMessage { get; set; }

        public DateTime LastMessageDate { get; set; }

        public string LastWriterUserId { get; set; }
    }
}
