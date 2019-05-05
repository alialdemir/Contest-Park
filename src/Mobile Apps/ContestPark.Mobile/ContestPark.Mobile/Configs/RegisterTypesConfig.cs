﻿using Autofac;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Blocking;
using ContestPark.Mobile.Services.Bot;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.CategoryFollow;
using ContestPark.Mobile.Services.Chat;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Follow;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.InAppBilling;
using ContestPark.Mobile.Services.Media;
using ContestPark.Mobile.Services.Mission;
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
            containerRegistry.RegisterForNavigation<AccountSettingsView, AccountSettingsViewModel>();

            containerRegistry.RegisterForNavigation<BlockingView, BlockingViewModel>();

            containerRegistry.RegisterForNavigation<BaseNavigationPage>();

            containerRegistry.RegisterForNavigation<CategoriesView, CategoriesViewModel>();

            containerRegistry.RegisterForNavigation<CategoryDetailView, CategoryDetailViewModel>();

            containerRegistry.RegisterForNavigation<SearchView, SearchViewModel>();

            containerRegistry.RegisterForNavigation<ChatDetailView, ChatDetailViewModel>();

            containerRegistry.RegisterForNavigation<ChatView, ChatViewModel>();

            containerRegistry.RegisterForNavigation<ContestStoreView, ContestStoreViewModel>();

            containerRegistry.RegisterForNavigation<DuelBettingPopupView, DuelBettingPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelResultPopupView, DuelResultPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelStartingPopupView, DuelStartingPopupViewModel>();

            containerRegistry.RegisterForNavigation<FollowersView, FollowersViewModel>();

            containerRegistry.RegisterForNavigation<ForgetYourPasswordView, ForgetYourPasswordViewModel>();

            containerRegistry.RegisterForNavigation<LanguageView, LanguageViewModel>();

            containerRegistry.RegisterForNavigation<MainView, MainPageViewModel>();

            containerRegistry.RegisterForNavigation<PostLikesView, PostLikesViewModel>();

            containerRegistry.RegisterForNavigation<PostDetailView, PostDetailViewModel>();

            containerRegistry.RegisterForNavigation<MasterDetailView, MasterDetailViewModel>();

            containerRegistry.RegisterForNavigation<MasterView, MasterViewModel>();

            containerRegistry.RegisterForNavigation<MissionsView, MissionsViewModel>();

            containerRegistry.RegisterForNavigation<NotificationsView, NotificationsViewModel>();

            containerRegistry.RegisterForNavigation<ProfileView, ProfileViewModel>();

            containerRegistry.RegisterForNavigation<QuestionExpectedPopupView, QuestionExpectedPopupViewModel>();

            containerRegistry.RegisterForNavigation<QuestionPopupView, QuestionPopupViewModel>();

            containerRegistry.RegisterForNavigation<RankingView, RankingViewModel>();

            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();

            containerRegistry.RegisterForNavigation<SignInView, SignInViewModel>();

            containerRegistry.RegisterForNavigation<SignUpView, SignUpViewModel>();

            containerRegistry.RegisterForNavigation<TabView, TabViewModel>();
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

                containerRegistry.RegisterSingleton<IBlockingService, BlockingService>();

                containerRegistry.RegisterSingleton<IPostService, PostService>();

                containerRegistry.RegisterSingleton<ICategoryService, CategoryServices>();

                containerRegistry.RegisterSingleton<ICategoryFollowService, CategoryFollowService>();

                containerRegistry.RegisterSingleton<IChatService, ChatService>();

                containerRegistry.RegisterSingleton<ICpService, CpService>();

                containerRegistry.RegisterSingleton<IMissionService, MissionService>();

                containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

                containerRegistry.RegisterSingleton<IDuelService, DuelService>();

                containerRegistry.RegisterSingleton<IFollowService, FollowService>();

                containerRegistry.RegisterSingleton<ISignalRServiceBase, SignalRServiceBase>();

                containerRegistry.RegisterSingleton<IDuelSignalRService, DuelSignalRService>();

                containerRegistry.RegisterSingleton<IScoreService, ScoreService>();

                containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            }
            else
            {
                containerRegistry.RegisterSingleton<IIdentityService, IdentityMockService>();

                containerRegistry.RegisterSingleton<IInAppBillingService, InAppBillingMockService>();

                containerRegistry.RegisterSingleton<IBlockingService, BlockingMockService>();

                containerRegistry.RegisterSingleton<IPostService, PostMockService>();

                containerRegistry.RegisterSingleton<ICategoryService, CategoryMockServices>();

                containerRegistry.RegisterSingleton<ICategoryFollowService, CategoryFollowMockService>();

                containerRegistry.RegisterSingleton<IChatService, ChatMockService>();

                containerRegistry.RegisterSingleton<ICpService, CpMockService>();

                containerRegistry.RegisterSingleton<IMissionService, MissionMockService>();

                containerRegistry.RegisterSingleton<INotificationService, NotificationMockService>();

                containerRegistry.RegisterSingleton<IDuelService, DuelMockService>();

                containerRegistry.RegisterSingleton<IFollowService, FollowMockService>();

                containerRegistry.RegisterSingleton<ISignalRServiceBase, SignalRMockServiceBase>();

                containerRegistry.RegisterSingleton<IDuelSignalRService, DuelSignalRMockService>();

                containerRegistry.RegisterSingleton<IScoreService, ScoreMockService>();

                containerRegistry.RegisterSingleton<ISettingsService, SettingsMockService>();
            }

            containerRegistry.RegisterSingleton<IBotService, BotService>();

            containerRegistry.RegisterSingleton<IGameService, GameService>();

            containerRegistry.RegisterSingleton<ICacheService, CacheService>();

            containerRegistry.Register<IAudioService, AudioService>();

            containerRegistry.Register<IMediaService, MediaService>();

            containerRegistry.RegisterInstance<IRequestProvider>(new RequestProviderFactory().CreateResilientHttpClient());
        }

        #endregion Register Instance
    }
}