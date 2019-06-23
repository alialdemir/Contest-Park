using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Conversation
{
    public interface IConversationRepository
    {
        Task<string> AddConversationAsync(string senderUserId, string receiverUserId);

        string GetConversationIdByParticipants(string senderUserId, string receiverUserId);
    }
}