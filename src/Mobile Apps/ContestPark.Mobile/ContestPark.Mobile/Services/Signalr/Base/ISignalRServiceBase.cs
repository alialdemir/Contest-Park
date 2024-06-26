﻿using System;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Signalr.Base
{
    public interface ISignalRServiceBase
    {
        bool IsConnected { get; }

        void On<T>(string methodName, Action<T> action);

        void Off(string methodName);

        Task ConnectAsync();

        Task DisconnectAsync();

        Task SendMessage(string methodName, params object[] param);
    }
}
