using ContestPark.Core.Database.Models;
using Dapper;
using System;

namespace ContestPark.Chat.API.Infrastructure.Mysql
{
    [Table("Conversations")]
    public class Conversation : EntityBase
    {
        [Key]
        public int ConversationId { get; set; }

        public string SenderUserId { get; set; }

        public string ReceiverUserId { get; set; }

        public int SenderUnreadMessageCount { get; set; }// bu konuşmadaki gönderenin okumadığı mesaj sayısı

        public int ReceiverUnreadMessageCount { get; set; }// bu konuşmadaki alıcının okumadığı  mesaj sayısı

        public string LastMessage { get; set; }

        public DateTime LastMessageDate { get; set; }

        public string LastWriterUserId { get; set; }
    }
}
