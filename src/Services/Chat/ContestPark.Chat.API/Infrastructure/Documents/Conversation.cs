using ContestPark.Core.CosmosDb.Models;
using System;

namespace ContestPark.Chat.API.Infrastructure.Documents
{
    public class Conversation : DocumentBase
    {
        public string SenderUserId { get; set; }

        public string ReceiverUserId { get; set; }

        public int SenderUnreadMessageCount { get; set; }// bu konuşmadaki gönderenin okumadığı mesaj sayısı

        public int ReceiverUnreadMessageCount { get; set; }// bu konuşmadaki alıcının okumadığı  mesaj sayısı

        public string LastMessage { get; set; }

        public DateTime LastMessageDate { get; set; }

        public string LastWriterUserId { get; set; }
    }
}