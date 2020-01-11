using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Bot;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Mission;
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

            containerRegistry.RegisterForNavigation<CategoriesView, CategoriesViewModel>();

            containerRegistry.RegisterForNavigation<CategoryDetailView, CategoryDetailViewModel>();

            containerRegistry.RegisterForNavigation<ChatDetailView, ChatDetailViewModel>();

            containerRegistry.RegisterForNavigation<ChatView, ChatViewModel>();

            containerRegistry.RegisterForNavigation<CheckSmsView, CheckSmsViewModel>();

            containerRegistry.RegisterForNavigation<ContestStoreView, ContestStoreViewModel>();

            containerRegistry.RegisterForNavigation<DuelBettingPopupView, DuelBettingPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelResultPopupView, DuelResultPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelStartingPopupView, DuelStartingPopupViewModel>();

            containerRegistry.RegisterForNavigation<FollowersView, FollowersViewModel>();

            containerRegistry.RegisterForNavigation<FollowingView, FollowingViewModel>();

            containerRegistry.RegisterForNavigation<ForgetYourPasswordView, ForgetYourPasswordViewModel>();

            containerRegistry.RegisterForNavigation<IbanNoView, IbanNoViewModel>();

            containerRegistry.RegisterForNavigation<InviteView, InviteViewModel>();

            containerRegistry.RegisterForNavigation<LanguageView, LanguageViewModel>();

            containerRegistry.RegisterForNavigation<MissionsView, MissionsViewModel>();

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

            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            containerRegistry.RegisterForNavigation<SignInView, SignInViewModel>();

            containerRegistry.RegisterForNavigation<SignUpFullNameView, SignUpFullNameViewModel>();

            containerRegistry.RegisterForNavigation<SignUpReferenceCodeView, SignUpReferenceCodeViewModel>();

            containerRegistry.RegisterForNavigation<SignUpUserNameView, SignUpUserNameViewModel>();

            containerRegistry.RegisterForNavigation<SignUpView, SignUpViewModel>();

            containerRegistry.RegisterForNavigation<WinningsView, WinningsViewModel>();
        }

        #endregion Navigation

        #region Register Instance

        private void RegisterTypeInstance(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();

            if (!GlobalSetting.Instance.IsMockData)
            {
                containerRegistry.Register<IIdentityService, IdentityService>();

                containerRegistry.Register<IInAppBillingService, InAppBillingService>();

                containerRegistry.Register<IBlockingService, BlockingService>();

                containerRegistry.Register<IPostService, PostService>();

                containerRegistry.Register<ICategoryService, CategoryServices>();

                containerRegistry.Register<IChatService, ChatService>();

                containerRegistry.Register<IBalanceService, BalanceService>();

                containerRegistry.Register<IMissionService, MissionService>();

                containerRegistry.Register<IDuelService, DuelService>();

                containerRegistry.Register<IFollowService, FollowService>();

                containerRegistry.Register<IRequestProvider, RequestProvider>();

                containerRegistry.Register<ISignalRServiceBase, SignalRServiceBase>();

                containerRegistry.Register<IDuelSignalRService, DuelSignalRService>();

                containerRegistry.Register<IScoreService, ScoreService>();
            }
            else
            {
                containerRegistry.Register<IIdentityService, IdentityMockService>();

                containerRegistry.Register<IInAppBillingService, InAppBillingMockService>();

                containerRegistry.Register<IBlockingService, BlockingMockService>();

                containerRegistry.Register<IPostService, PostMockService>();

                containerRegistry.Register<ICategoryService, CategoryMockServices>();

                containerRegistry.Register<IChatService, ChatMockService>();

                containerRegistry.Register<IBalanceService, BalanceMockService>();

                containerRegistry.Register<IMissionService, MissionMockService>();

                containerRegistry.Register<IDuelService, DuelMockService>();

                containerRegistry.Register<IFollowService, FollowMockService>();

                containerRegistry.Register<IRequestProvider, RequestProvider>();

                containerRegistry.Register<ISignalRServiceBase, SignalRMockServiceBase>();

                containerRegistry.Register<IDuelSignalRService, DuelSignalRMockService>();

                containerRegistry.Register<IScoreService, ScoreMockService>();
            }

            containerRegistry.Register<ISettingsService, SettingsService>();

            containerRegistry.Register<IBotService, BotService>();

            containerRegistry.Register<IGameService, GameService>();

            containerRegistry.Register<ICacheService, CacheService>();

            containerRegistry.Register<IAudioService, AudioService>();

            containerRegistry.Register<IMediaService, MediaService>();

            containerRegistry.Register<IAdMobService, AdMobService>();

            containerRegistry.Register<IAnalyticsService, AnalyticsService>();

            //containerRegistry.RegisterInstance<IRequestProvider>(new RequestProviderFactory().CreateResilientHttpClient());
        }

        #endregion Register Instance
    }
}
