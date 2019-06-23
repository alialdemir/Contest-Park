﻿using ContestPark.Core.CosmosDb.Models;

namespace ContestPark.Chat.API.Infrastructure.Documents
{
    public class Message : DocumentBase
    {
        public string ConversationId { get; set; }

        public string Text { get; set; }

        public string AuthorUserId { get; set; }
    }
}