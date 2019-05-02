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

        Task<bool> DeleteAsync(string receiverUserId);

        Task<int> UserChatVisibilityCountAsync();
        Task<ServiceModel<ChatDetailModel>> ChatDetailAsync(string senderUserId, PagingModel pagingModel);
    }
}