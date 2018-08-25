using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Base
{
    public interface ISignalRServiceBase
    {
        void On<T>(string methodName, Action<T> action);

        void Off(string methodName);

        Task ConnectAsync();

        Task DisconnectAsync();

        Task Init();
    }
}