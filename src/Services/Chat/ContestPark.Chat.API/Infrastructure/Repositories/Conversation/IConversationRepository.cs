using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public interface IConversationRepository
    {
        Task<bool> AllMessagesRead(string useerId, long conversationId);

        bool IsConversationBelongUser(string userId, long conversationId);

        int UnReadMessageCount(string userId);

        ServiceModel<MessageModel> UserMessages(string userId, PagingModel paging);
    }
}
