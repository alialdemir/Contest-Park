using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
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
using ContestPark.Mobile.ViewModels;
using ContestPark.Mobile.Views;
using Prism.Ioc;
using Prism.Plugin.Popups;

namespace ContestPark.Mobile.Configs
{
    public static class RegisterTypesConfig
    {
        #region Navigation

        public static void RegisterTypeForNavigation(this IContainerRegistry containerRegistry)
        {
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

            containerRegistry.RegisterForNavigation<SpecialOfferPopupView, SpecialOfferPopupViewModel>();

            containerRegistry.RegisterForNavigation<TutorialPopupView, TutorialPopupViewModel>();

            containerRegistry.RegisterForNavigation<AcceptDuelInvitationPopupView, AcceptDuelInvitationPopupViewModel>();
        }

        #endregion Navigation

        #region Register Instance

        public static void RegisterTypeInstance(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.RegisterSingleton<IIdentityService, IdentityService>();

            containerRegistry.RegisterSingleton<IInAppBillingService, InAppBillingService>();

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

            containerRegistry.RegisterSingleton<ILatestVersionService, LatestVersionService>();

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
