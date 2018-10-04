using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Base
{
    public class SignalRMockServiceBase : ISignalRServiceBase
    {
        public bool IsConnect => true;

        public Task ConnectAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            return Task.CompletedTask;
        }

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public void Off(string methodName)
        {
        }

        public void On<T>(string methodName, Action<T> action)
        {
        }

        public Task SendMessage(string methodName, params object[] param)
        {
            return Task.CompletedTask;
        }
    }
}