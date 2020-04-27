using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.BackgroundAggregator;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.Services.InviteDuel;
using ContestPark.Mobile.Services.LatestVersion;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Mission;
using ContestPark.Mobile.Services.Notice;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.Services.RequestProvider;
using ContestPark.Mobile.Services.Score;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Base;
using ContestPark.Mobile.Services.Signalr.Duel;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.IO;
using Shiny.Notifications;
using Shiny.Prism;

//using Shiny.Push;
using SQLite;
using System;

//using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Shiny
{
    public class ShinyAppStartup : PrismStartup
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            //Enable your shiny services here

            services.AddSingleton<IIdentityService, IdentityService>();

            services.AddSingleton<IInAppBillingService, InAppBillingService>();

            services.AddSingleton<IBackgroundAggregatorService, BackgroundAggregatorService>();

            services.AddSingleton<IBlockingService, BlockingService>();

            services.AddSingleton<IPostService, PostService>();

            services.AddSingleton<ICategoryService, CategoryServices>();

            services.AddSingleton<IChatService, ChatService>();

            services.AddSingleton<IBalanceService, BalanceService>();

            services.AddSingleton<IMissionService, MissionService>();

            services.AddSingleton<INoticeService, NoticeService>();

            services.AddSingleton<INotificationService, NotificationService>();

            services.AddSingleton<IDuelService, DuelService>();

            services.AddSingleton<IFollowService, FollowService>();

            services.AddSingleton<IRequestProvider, RequestProvider.RequestProvider>();

            services.AddSingleton<ISignalRServiceBase, SignalRServiceBase>();

            services.AddSingleton<IDuelSignalRService, DuelSignalRService>();

            services.AddSingleton<IScoreService, ScoreService>();

            services.AddSingleton<ILatestVersionService, LatestVersionService>();

            services.AddSingleton<ISettingsService, SettingsService>();

            services.AddSingleton<IGameService, GameService>();

            services.AddSingleton<ICacheService, CacheService>();

            services.AddSingleton<IAudioService, AudioService>();

            services.AddSingleton<IMediaService, MediaService>();

            services.AddSingleton<IAdMobService, AdMobService>();

            services.AddSingleton<IAnalyticsService, AnalyticsService>();

            services.AddSingleton<IInviteDuelService, InviteDuelService>();

            // services.UseFirebaseMessaging<PushDelegate>();
            services.UseNotifications<NotificationDelegate>(
          true,
          new NotificationCategory(
              "Test",
              new NotificationAction("Reply", "Reply", NotificationActionType.TextReply),
              new NotificationAction("Yes", "Yes", NotificationActionType.None),
              new NotificationAction("No", "No", NotificationActionType.Destructive)
          )
      );
        }
    }

    public class NotificationEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int NotificationId { get; set; }
        public string NotificationTitle { get; set; }
        public string Action { get; set; }
        public string ReplyText { get; set; }
        public string Payload { get; set; }
        public bool IsEntry { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NotificationDelegate : INotificationDelegate
    {
        private readonly SampleSqliteConnection conn;
        private readonly IMessageBus messageBus;
        private readonly INotificationManager notifications;

        public NotificationDelegate(SampleSqliteConnection conn, IMessageBus messageBus, INotificationManager notifications)
        {
            this.conn = conn;
            this.messageBus = messageBus;
            this.notifications = notifications;
        }

        public Task OnEntry(NotificationResponse response) => this.Store(new NotificationEvent
        {
            NotificationId = response.Notification.Id,
            NotificationTitle = response.Notification.Title ?? response.Notification.Message,
            Action = response.ActionIdentifier,
            ReplyText = response.Text,
            IsEntry = true,
            Timestamp = DateTime.Now
        });

        public Task OnReceived(global::Shiny.Notifications.Notification notification) => this.Store(new NotificationEvent
        {
            NotificationId = notification.Id,
            NotificationTitle = notification.Title ?? notification.Message,
            IsEntry = false,
            Timestamp = DateTime.Now
        });

        private async Task Store(NotificationEvent @event)
        {
            await this.conn.InsertAsync(@event);
            this.messageBus.Publish(@event);
        }
    }

    //public class PushDelegate : IPushDelegate
    //{
    //    private readonly CoreDelegateServices services;
    //    private readonly IPushManager pushManager;

    //    public PushDelegate(CoreDelegateServices services, IPushManager pushManager)
    //    {
    //        this.services = services;
    //        this.pushManager = pushManager;
    //    }

    //    public Task OnEntry(PushEntryArgs args)
    //        => this.Insert("PUSH ENTRY");

    //    public Task OnReceived(IDictionary<string, string> data)
    //        => this.Insert("PUSH RECEIVED");

    //    public Task OnTokenChanged(string token)
    //        => this.Insert("PUSH TOKEN CHANGE");

    //    private Task Insert(string info) => this.services.Connection.InsertAsync(new PushEvent
    //    {
    //        Payload = info,
    //        Token = this.pushManager.CurrentRegistrationToken,
    //        Timestamp = DateTime.UtcNow
    //    });
    //}

    public class PushEvent
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public string Token { get; set; }
        public string Payload { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CoreDelegateServices
    {
        public CoreDelegateServices(SampleSqliteConnection conn,
                                    INotificationManager notifications)
        {
            this.Connection = conn;
            this.Notifications = notifications;
        }

        public SampleSqliteConnection Connection { get; }
        public INotificationManager Notifications { get; }

        public async Task SendNotification(string title, string message)
        {
            var notify = true;
            if (notify)
                await this.Notifications.Send(title, message);
        }
    }

    public class SampleSqliteConnection : SQLiteAsyncConnection
    {
        public SampleSqliteConnection(IFileSystem fileSystem) : base(Path.Combine(fileSystem.AppData.FullName, "ContestPark.db"))
        {
            var conn = this.GetConnection();
            conn.CreateTable<PushEvent>();
        }

        public AsyncTableQuery<PushEvent> PushEvents => this.Table<PushEvent>();
    }
}
