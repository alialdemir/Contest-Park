using ContestPark.Mobile.Models.Chat;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Chat
{
    public interface IChatService
    {
        Task<ServiceModel<ChatModel>> UserChatList(PagingModel pagingModel);

        Task<bool> ChatSeenAsync();

        Task<bool> DeleteAsync(long conversationId);

        Task<int> UserChatVisibilityCountAsync();

        Task<ServiceModel<ChatDetailModel>> ChatDetailAsync(long conversationId, PagingModel pagingModel);

        Task<bool> SendMessage(MessageModel messageModel);
    }
}
