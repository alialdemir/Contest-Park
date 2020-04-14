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
using ContestPark.Mobile.ViewModels;
using ContestPark.Mobile.Views;
using Prism.Ioc;
using Prism.Plugin.Popups;

namespace ContestPark.Mobile.Configs
{
    public class RegisterTypesConfig
    {
        #region Container

        public static IContainerProvider Container { get; set; }

        #endregion Container

        #region Init

        public static void Init(IContainerProvider container, IContainerRegistry containerRegistry)
        {
            Container = container;

            RegisterTypesConfig config = new RegisterTypesConfig();

            config.RegisterTypeInstance(containerRegistry);
            config.RegisterTypeForNavigation(containerRegistry);
        }

        #endregion Init

        #region Navigation

        private void RegisterTypeForNavigation(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BaseNavigationPage>();

            containerRegistry.RegisterForNavigation<Xamarin.Forms.NavigationPage>();

            containerRegistry.RegisterForNavigation<AccountSettingsView, AccountSettingsViewModel>();

            containerRegistry.RegisterForNavigation<AppShell, AppShellViewModel>();

            containerRegistry.RegisterForNavigation<BalanceCodeView, BalanceCodeViewModel>();

            containerRegistry.RegisterForNavigation<BlockingView, BlockingViewModel>();

            containerRegistry.RegisterForNavigation<BrowserView, BrowserViewModel>();

            containerRegistry.RegisterForNavigation<CategoriesView, CategoriesViewModel>();

            containerRegistry.RegisterForNavigation<CategoryDetailView, CategoryDetailViewModel>();

            containerRegistry.RegisterForNavigation<ChatDetailView, ChatDetailViewModel>();

            containerRegistry.RegisterForNavigation<ChatView, ChatViewModel>();

            containerRegistry.RegisterForNavigation<SignUpVerificationView, SignUpVerificationViewModel>();

            containerRegistry.RegisterForNavigation<ContestStoreView, ContestStoreViewModel>();

            containerRegistry.RegisterForNavigation<DuelBettingPopupView, DuelBettingPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelResultPopupView, DuelResultPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelStartingPopupView, DuelStartingPopupViewModel>();

            containerRegistry.RegisterForNavigation<FollowersView, FollowersViewModel>();

            containerRegistry.RegisterForNavigation<FollowingView, FollowingViewModel>();

            containerRegistry.RegisterForNavigation<GiftGoldPopupView, GiftGoldPopupViewModel>();

            containerRegistry.RegisterForNavigation<IbanNoView, IbanNoViewModel>();

            containerRegistry.RegisterForNavigation<InviteView, InviteViewModel>();

            containerRegistry.RegisterForNavigation<LanguageView, LanguageViewModel>();

            containerRegistry.RegisterForNavigation<LeaderBoardView, LeaderBoardViewViewModel>();

            containerRegistry.RegisterForNavigation<MissionsView, MissionsViewModel>();

            containerRegistry.RegisterForNavigation<NotificationView, NotificationViewModel>();

            containerRegistry.RegisterForNavigation<PhoneNumberView, PhoneNumberViewModel>();

            containerRegistry.RegisterForNavigation<PhotoModalView, PhotoModalViewModel>();

            containerRegistry.RegisterForNavigation<PostDetailView, PostDetailViewModel>();

            containerRegistry.RegisterForNavigation<PostLikesView, PostLikesViewModel>();

            containerRegistry.RegisterForNavigation<ProfileView, ProfileViewModel>();

            containerRegistry.RegisterForNavigation<MyProfileView, ProfileViewModel>();

            containerRegistry.RegisterForNavigation<QuestionExpectedPopupView, QuestionExpectedPopupViewModel>();

            containerRegistry.RegisterForNavigation<QuestionPopupView, QuestionPopupViewModel>();

            containerRegistry.RegisterForNavigation<RankingView, RankingViewModel>();

            containerRegistry.RegisterForNavigation<SearchView, SearchViewModel>();

            containerRegistry.RegisterForNavigation<SelectCountryView, SelectCountryViewModel>();

            containerRegistry.RegisterForNavigation<SelectSubCategoryView, SelectSubCategoryViewModel>();

            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            containerRegistry.RegisterForNavigation<SignUpFullNameView, SignUpFullNameViewModel>();

            containerRegistry.RegisterForNavigation<SignUpReferenceCodeView, SignUpReferenceCodeViewModel>();

            containerRegistry.RegisterForNavigation<SignUpSelectSubCategoriesView, SignUpSelectSubCategoriesViewModel>();

            containerRegistry.RegisterForNavigation<SignUpUserNameView, SignUpUserNameViewModel>();

            containerRegistry.RegisterForNavigation<WinningsView, WinningsViewModel>();

            containerRegistry.RegisterForNavigation<TutorialPopupView, TutorialPopupViewModel>();

            containerRegistry.RegisterForNavigation<AcceptDuelInvitationPopupView, AcceptDuelInvitationPopupViewModel>();
        }

        #endregion Navigation

        #region Register Instance

        private void RegisterTypeInstance(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();

            if (!GlobalSetting.Instance.IsMockData)
            {
                containerRegistry.RegisterSingleton<IIdentityService, IdentityService>();

                containerRegistry.RegisterSingleton<IInAppBillingService, InAppBillingService>();

                containerRegistry.RegisterSingleton<IBackgroundAggregatorService, BackgroundAggregatorService>();

                containerRegistry.RegisterSingleton<IBlockingService, BlockingService>();

                containerRegistry.RegisterSingleton<IPostService, PostService>();

                containerRegistry.RegisterSingleton<ICategoryService, CategoryServices>();

                containerRegistry.RegisterSingleton<IChatService, ChatService>();

                containerRegistry.RegisterSingleton<IBalanceService, BalanceService>();

                containerRegistry.RegisterSingleton<IMissionService, MissionService>();

                containerRegistry.RegisterSingleton<INoticeService, NoticeService>();

                containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

                containerRegistry.RegisterSingleton<IDuelService, DuelService>();

                containerRegistry.RegisterSingleton<IFollowService, FollowService>();

                containerRegistry.RegisterSingleton<IRequestProvider, RequestProvider>();

                containerRegistry.RegisterSingleton<ISignalRServiceBase, SignalRServiceBase>();

                containerRegistry.RegisterSingleton<IDuelSignalRService, DuelSignalRService>();

                containerRegistry.RegisterSingleton<IScoreService, ScoreService>();
            }
            else
            {
                containerRegistry.RegisterSingleton<IIdentityService, IdentityMockService>();

                containerRegistry.RegisterSingleton<IInAppBillingService, InAppBillingMockService>();

                containerRegistry.RegisterSingleton<IBlockingService, BlockingMockService>();

                containerRegistry.RegisterSingleton<IPostService, PostMockService>();

                containerRegistry.RegisterSingleton<ICategoryService, CategoryMockServices>();

                containerRegistry.RegisterSingleton<IChatService, ChatMockService>();

                containerRegistry.RegisterSingleton<IBalanceService, BalanceMockService>();

                containerRegistry.RegisterSingleton<IMissionService, MissionMockService>();

                containerRegistry.RegisterSingleton<INotificationService, NotificationMockService>();

                containerRegistry.RegisterSingleton<IDuelService, DuelMockService>();

                containerRegistry.RegisterSingleton<IFollowService, FollowMockService>();

                containerRegistry.RegisterSingleton<IRequestProvider, RequestProvider>();

                containerRegistry.RegisterSingleton<ISignalRServiceBase, SignalRMockServiceBase>();

                containerRegistry.RegisterSingleton<IDuelSignalRService, DuelSignalRMockService>();

                containerRegistry.RegisterSingleton<IScoreService, ScoreMockService>();
            }

            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();

            containerRegistry.RegisterSingleton<IGameService, GameService>();

            containerRegistry.RegisterSingleton<ICacheService, CacheService>();

            containerRegistry.RegisterSingleton<IAudioService, AudioService>();

            containerRegistry.RegisterSingleton<IMediaService, MediaService>();

            containerRegistry.RegisterSingleton<IAdMobService, AdMobService>();

            containerRegistry.RegisterSingleton<IAnalyticsService, AnalyticsService>();

            containerRegistry.RegisterSingleton<IInviteDuelService, InviteDuelService>();

            //containerRegistry.RegisterInstance<IRequestProvider>(new RequestProviderFactory().CreateResilientHttpClient());
        }

        #endregion Register Instance
    }
}
