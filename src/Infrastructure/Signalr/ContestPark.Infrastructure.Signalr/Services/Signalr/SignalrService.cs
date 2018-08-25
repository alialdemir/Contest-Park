//using ContestPark.Domain.Signalr.Model.Request;
//using Microsoft.AspNetCore.SignalR.Client;
//using System.Diagnostics;
//using System.Threading.Tasks;

//namespace ContestPark.Infrastructure.Signalr.Services.Signalr
//{
//    internal class SignalrService : ISignalrService
//    {
//        #region Properties

//        private int ConnectionRetryCount { get; set; }

//        private HubConnection _hubConnection;

//        #endregion Properties

//        #region Constructor

//        public SignalrService()
//        {
//            Init();
//        }

//        #endregion Constructor

//        #region Methods

//        /// <summary>
//        /// SignalR servisini oluştur
//        /// </summary>
//        public async Task Init()
//        {
//            _hubConnection = new HubConnectionBuilder()
//                .WithUrl("http://169.254.80.80:5104/contestpark", (options) =>
//                {
//                    options.Headers.Add("IsGrain", "true");
//                }).Build();

//            await ConnectAsync();
//        }

//        /// <summary>
//        /// SignalR bağlantı aç
//        /// </summary>
//        public async Task ConnectAsync()
//        {
//            await _hubConnection
//                .StartAsync()
//                .ContinueWith(async (task) =>
//                {
//                    if (!task.IsFaulted && ConnectionRetryCount != 0)
//                    {
//                        ConnectionRetryCount = 0;
//                    }
//#if DEBUG
//                    else
//                    {
//                        ConnectionRetryCount++;
//                        Debug.WriteLine($"Signalr bağlanmaya çalışılıyor. Retry count: {ConnectionRetryCount}");
//                        await Task.Delay(3000);
//                        Task.Run(async () => await ConnectAsync());
//                    }
//#else
//                                 else if (task.IsFaulted && ConnectionRetryCount < 10)
//                                {
//                                    ConnectionRetryCount++;
//                                    await Task.Delay(3000);
//                                    await ConnectAsync();
//                                }
//#endif
//                });
//        }

//        /// <summary>
//        /// Cliente messajı göndermek için ContestPark.Signalr apisine mesajı gönderir
//        /// </summary>
//        /// <param name = "message" > Cliente Gönderilecek data</param>
//        public async Task SendMessage(string methodName, string connectionId, object param)
//        {
//            await _hubConnection.SendAsync("SendMessage", new SendMessage
//            {
//                ConnectionId = connectionId,
//                Param = param,
//                MethodName = methodName
//            });
//        }

//        #endregion Methods
//    }
//}