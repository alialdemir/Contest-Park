using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Services.Settings;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ContestPark.Mobile.Services.Signalr.Base
{
    public class SignalRServiceBase : ISignalRServiceBase
    {
        #region Constructor

        public SignalRServiceBase(ISettingsService settingsService,
                                  IEventAggregator eventAggregator)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        private HubConnection HubConnection { get; set; }

        private readonly ISettingsService _settingsService;
        private readonly IEventAggregator _eventAggregator;
        private int ConnectionRetryCount;

        private Dictionary<string, IDisposable> DisposableOns { get; } = new Dictionary<string, IDisposable>();

        public bool IsConnected
        {
            get
            {
                bool isConnect = HubConnection == null || HubConnection.State == HubConnectionState.Disconnected;
                if (isConnect)
                    Init();

                return !isConnect;
            }
        }

        #endregion Properties

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
        }

        /// <summary>
        /// SignalR bağlantı aç
        /// </summary>
        public async Task ConnectAsync()
        {
            if (HubConnection.State != HubConnectionState.Disconnected)
                return;

            await HubConnection
                   .StartAsync()
                    .ContinueWith(async (task) =>
                        {
                            if (!task.IsFaulted && ConnectionRetryCount != 0)
                            {
                                ConnectionRetryCount = 0;
                            }
#if DEBUG
                            else if (HubConnection.State == HubConnectionState.Disconnected)
                            {
                                ConnectionRetryCount++;
                                Debug.WriteLine($"Signalr bağlanmaya çalışılıyor. Retry count: {ConnectionRetryCount}");
                                await Task.Delay(500);
                                await ConnectAsync();
                            }
#else
                                 else if (task.IsFaulted && ConnectionRetryCount < 10 && HubConnection.State == HubConnectionState.Disconnected)
                                {
                                    ConnectionRetryCount++;
                                    await Task.Delay(8000);
                                    await ConnectAsync();
                                }
#endif
                        });
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

                    _eventAggregator
                            .GetEvent<SignalrConnectedEvent>()
                            .Publish(connectionId);
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
