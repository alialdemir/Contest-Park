using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public interface IConversationRepository
    {
        Task<Documents.Conversation> AddOrGetConversationAsync(string senderUserId, string receiverUserId);
        Task<bool> AllMessagesRead(string useerId, string conversationId);
        bool IsConversationBelongUser(string userId, string conversationId);

        bool IsSender(string userId, string conversationId);
        int UnReadMessageCount(string userId);
        Task<bool> UpdateAsync(Documents.Conversation conversation);
        Core.CosmosDb.Models.ServiceModel<Model.MessageModel> UserMessages(string userId, Core.CosmosDb.Models.PagingModel paging);
    }
}