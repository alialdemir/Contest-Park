using ContestPark.Chat.API.Model;
using ContestPark.Core.Database.Models;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public interface IMessageRepository
    {
        long AddMessage(SendMessageModel message);

        ServiceModel<ConversationDetailModel> ConversationDetail(string userId, long conversationId, PagingModel paging);

        Task<bool> RemoveMessagesAsync(string userId, long conversationId);
    }
}
