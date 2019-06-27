using ContestPark.Chat.API.Model;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public interface IMessageRepository
    {
        Task<bool> AddMessage(SendMessageModel message);
        Core.CosmosDb.Models.ServiceModel<ConversationDetailModel> ConversationDetail(string conversationId, Core.CosmosDb.Models.PagingModel paging);
        Task<bool> RemoveMessages(string userId, string conversationId);
    }
}