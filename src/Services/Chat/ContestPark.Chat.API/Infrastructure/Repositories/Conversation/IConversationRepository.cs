﻿using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public interface IConversationRepository
    {
        Task<Documents.Conversation> AddOrGetConversationAsync(string senderUserId, string receiverUserId);

        bool IsConversationBelongUser(string userId, string conversationId);

        bool IsSender(string userId, string conversationId);

        Task<bool> UpdateAsync(Documents.Conversation conversation);
    }
}