using ContestPark.Chat.API.Model;
using System.Threading.Tasks;

namespace ContestPark.Chat.API.Infrastructure.Repositories.Message
{
    public interface IMessageRepository
    {
        Task<bool> AddMessage(MessageModel message);
    }
}