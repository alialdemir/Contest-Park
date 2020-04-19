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
using Shiny.Prism;
using Shiny.Push;
using System.Collections.Generic;
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

            services.AddSingleton<IRequestProvider, ContestPark.Mobile.Services.RequestProvider.RequestProvider>();

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

            services.UseFirebaseMessaging<PushDelegate>();

            services.UseNotifications(true);
        }
    }

    public class PushDelegate : IPushDelegate
    {
        private readonly IPushManager pushManager;

        public PushDelegate(IPushManager pushManager)
        {
            this.pushManager = pushManager;
        }

        public Task OnEntry(PushEntryArgs args)
            => this.Insert("PUSH ENTRY");

        public Task OnReceived(IDictionary<string, string> data)
            => this.Insert("PUSH RECEIVED");

        public Task OnTokenChanged(string token)
            => this.Insert(token);

        private Task Insert(string info)
        {
            //this.services.Connection.InsertAsync(new PushEvent
            //{
            //    Payload = info,
            //    Token = this.pushManager.CurrentRegistrationToken,
            //    Timestamp = DateTime.UtcNow
            //});

            return Task.CompletedTask;
        }
    }
}
