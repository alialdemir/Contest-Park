using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Signalr.Services.Signalr
{
    public interface ISignalrService
    {
        Task SendMessage(string methodName, string connectionId, object param);
    }
}