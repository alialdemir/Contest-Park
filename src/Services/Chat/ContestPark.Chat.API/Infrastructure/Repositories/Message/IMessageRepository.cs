using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public interface IMessageRepository
    {
        Task<bool> AddMessage(SendMessageModel message);
        ServiceModel<ConversationDetailModel> ConversationDetail(string conversationId, PagingModel paging);
        Task<bool> RemoveMessages(string userId, string conversationId);
    }
}