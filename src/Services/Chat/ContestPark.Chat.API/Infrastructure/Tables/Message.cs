﻿using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Chat.API.Infrastructure.Tables
{
    [Table("Messages")]
    public class Message : EntityBase
    {
        [Key]
        public long MessageId { get; set; }

        public long ConversationId { get; set; }

        public string Text { get; set; }

        public string AuthorUserId { get; set; }

        public bool ReceiverIsDeleted { get; set; }

        public bool SenderIsDeleted { get; set; }

        public bool ReceiverIsReadMessage { get; set; }
    }
}
