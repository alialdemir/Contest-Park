using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public interface IConversationRepository
    {
        Task<Documents.Conversation> AddOrGetConversationAsync(string senderUserId, string receiverUserId);

        Task<bool> UpdateAsync(Documents.Conversation conversation);
    }
}