using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Services.Settings;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.Signalr.Base
{
    public class SignalRServiceBase : ISignalRServiceBase
    {
        #region Properties

        private int ConnectionRetryCount { get; set; }

        private HubConnection HubConnection;

        private readonly ISettingsService _settingsService;

        private Dictionary<string, IDisposable> DisposableOns { get; } = new Dictionary<string, IDisposable>();

        public bool IsConnected
        {
            get
            {
                bool isConnect = HubConnection != null && HubConnection.State == HubConnectionState.Connected && !string.IsNullOrEmpty(_settingsService.SignalRConnectionId);
                if (!isConnect)
                    Init();

                return isConnect;
            }
        }

        #endregion Properties

        #region Constructor

        public SignalRServiceBase(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// SignalR servisini oluştur
        /// </summary>
        public async Task Init()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return;

            HubConnection = new HubConnectionBuilder()
                .WithUrl(GlobalSetting.Instance.SignalREndpoint, (options) =>
            {
                options.AccessTokenProvider = () => Task.Run(() => _settingsService.AuthAccessToken);
            }).Build();

            HubConnection.ServerTimeout = TimeSpan.FromMinutes(60);// Signalr timeout süresi arıtrıldı
            HubConnection.HandshakeTimeout = TimeSpan.FromMinutes(60);// Signalr timeout süresi arıtrıldı

            GetConnection();

            RemoveConnectionId();

            await ConnectAsync();

            HubConnection.Closed += HubConnection_Closed;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            //    _settingsService.SignalRConnectionId = string.Empty;

            return Task.CompletedTask;
        }

        /// <summary>
        /// SignalR bağlantı aç
        /// </summary>
        public async Task ConnectAsync()
        {
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection
                       .StartAsync();
            }
            //                .ContinueWith(async (task) =>
            //                {
            //                    if (!task.IsFaulted && ConnectionRetryCount != 0)
            //                    {
            //                        ConnectionRetryCount = 0;
            //                    }
            //#if DEBUG
            //                    else if (HubConnection.State == HubConnectionState.Disconnected)
            //                    {
            //                        ConnectionRetryCount++;
            //                        Debug.WriteLine($"Signalr bağlanmaya çalışılıyor. Retry count: {ConnectionRetryCount}");
            //                        await Task.Delay(500);
            //                        await ConnectAsync();
            //                    }
            //#else
            //                     else if (task.IsFaulted && ConnectionRetryCount < 10 && HubConnection.State == HubConnectionState.Disconnected)
            //                    {
            //                        ConnectionRetryCount++;
            //                        await Task.Delay(8000);
            //                        await ConnectAsync();
            //                    }
            //#endif
            //                });
        }

        /// <summary>
        /// SignalR bağlantı kapat
        /// </summary>
        public async Task DisconnectAsync()
        {
            await HubConnection?.StopAsync();
        }

        /// <summary>
        /// Signalr event dinleme bir eventi bir sefer dinleyebiliriz
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="action"></param>
        public void On<T>(string methodName, Action<T> action)
        {
            IDisposable disposable = HubConnection?.On<T>(methodName, action);

            if (!IsContainsKey(methodName))
            {
                DisposableOns?.Add(methodName, disposable);
            }
        }

        public async Task SendMessage(string methodName, params object[] param)
        {
            await HubConnection?.InvokeCoreAsync(methodName, param);
        }

        /// <summary>
        /// Signalr bağlantısını kapatır
        /// </summary>
        /// <param name="methodName">Bağlantı adı</param>
        public void Off(string methodName)
        {
            if (!IsContainsKey(methodName)) return;

            DisposableOns[methodName].Dispose();

            DisposableOns.Remove(methodName);
        }

        private bool IsContainsKey(string key)
        {
            if (DisposableOns == null)
                return false;

            return DisposableOns.ContainsKey(key);
        }

        /// <summary>
        /// Serverde oluşan connection id clienten almak için dinler
        /// </summary>
        private void GetConnection()
        {
            HubConnection?.On("GetConnectionId", (string connectionId) =>
                {
                    _settingsService.SignalRConnectionId = connectionId;
                });
        }

        /// <summary>
        /// Serverde connection id clienten siler
        /// </summary>
        private void RemoveConnectionId()
        {
            HubConnection?.On("RemoveConnectionId", (string connectionId) =>
            {
                _settingsService.SignalRConnectionId = "";
            });
        }

        #endregion Methods
    }
}
