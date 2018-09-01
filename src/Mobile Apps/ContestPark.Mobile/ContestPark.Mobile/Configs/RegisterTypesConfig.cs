using Autofac;
using ContestPark.Mobile.Services.Audio;
using ContestPark.Mobile.Services.Bot;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Duel;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.RequestProvider;
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

            containerRegistry.RegisterForNavigation<CategoriesView, CategoriesViewModel>();

            containerRegistry.RegisterForNavigation<CategoryDetailView, CategoryDetailViewModel>();

            containerRegistry.RegisterForNavigation<DuelBettingPopupView, DuelBettingPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelResultPopupView, DuelResultPopupViewModel>();

            containerRegistry.RegisterForNavigation<DuelStartingPopupView, DuelStartingPopupViewModel>();

            containerRegistry.RegisterForNavigation<ForgetYourPasswordView, ForgetYourPasswordViewModel>();

            containerRegistry.RegisterForNavigation<MainView, MainPageViewModel>();

            containerRegistry.RegisterForNavigation<MasterDetailView, MasterDetailViewModel>();

            containerRegistry.RegisterForNavigation<MasterView, MasterViewModel>();

            containerRegistry.RegisterForNavigation<QuestionExpectedPopupView, QuestionExpectedPopupViewModel>();

            containerRegistry.RegisterForNavigation<QuestionPopupView, QuestionPopupViewModel>();

            containerRegistry.RegisterForNavigation<SignInView, SignInViewModel>();

            containerRegistry.RegisterForNavigation<SignUpView, SignUpViewModel>();

            containerRegistry.RegisterForNavigation<TabView, TabViewModel>();
        }

        #endregion Navigation

        #region Register Instance

        private void RegisterTypeInstance(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.Register<IIdentityService, IdentityService>();

            containerRegistry.Register<IBotService, BotService>();

            containerRegistry.Register<ICategoryServices, CategoryServices>();

            containerRegistry.Register<ICpService, CpService>();

            containerRegistry.RegisterSingleton<ICacheService, CacheService>();

            containerRegistry.Register<IAudioService, AudioService>();

            containerRegistry.RegisterSingleton<IGameervice, GameService>();

            containerRegistry.RegisterSingleton<IDuelService, DuelService>();

            containerRegistry.RegisterSingleton<ISignalRServiceBase, SignalRServiceBase>();

            containerRegistry.RegisterSingleton<IDuelSignalRService, DuelSignalRService>();

            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();

            containerRegistry.RegisterInstance<IRequestProvider>(new RequestProviderFactory().CreateResilientHttpClient());

            /*
            containerBuilder.RegisterType<RequestProvider>().As<IRequestProvider>().SingleInstance();
            containerBuilder.RegisterType<ChatsSignalRService>().As<IChatsSignalRService>().SingleInstance();

                containerBuilder.RegisterType<AccountService>().As<IAccountService>().SingleInstance();
                containerBuilder.RegisterType<BoostsService>().As<IBoostsService>().SingleInstance();
                containerBuilder.RegisterType<ChatBlocksService>().As<IChatBlocksService>().SingleInstance();
                containerBuilder.RegisterType<ChatRepliesService>().As<IChatRepliesService>().SingleInstance();
                containerBuilder.RegisterType<ChatsService>().As<IChatsService>().SingleInstance();
                containerBuilder.RegisterType<CommentsService>().As<ICommentsService>().SingleInstance();
                containerBuilder.RegisterType<CategoryServices>().As<ICategoryServices>().SingleInstance();
                containerBuilder.RegisterType<ContestDatesService>().As<IContestDatesService>().SingleInstance();
                containerBuilder.RegisterType<CoverPicturesService>().As<ICoverPicturesService>().SingleInstance();
                containerBuilder.RegisterType<CpService>().As<ICpService>().SingleInstance();
                containerBuilder.RegisterType<DuelInfosService>().As<IDuelInfosService>().SingleInstance();
                containerBuilder.RegisterType<DuelSignalRService>().As<IDuelSignalRService>().SingleInstance();
                containerBuilder.RegisterType<FollowCategoryService>().As<IFollowCategoryService>().SingleInstance();
                containerBuilder.RegisterType<FollowsService>().As<IFollowsService>().SingleInstance();
                containerBuilder.RegisterType<LanguageService>().As<ILanguageService>().SingleInstance();
                containerBuilder.RegisterType<LikesService>().As<ILikesService>().SingleInstance();
                containerBuilder.RegisterType<MissionsService>().As<IMissionsService>().SingleInstance();
                containerBuilder.RegisterType<NotificationsService>().As<INotificationsService>().SingleInstance();
                containerBuilder.RegisterType<OpenSubCategoryService>().As<IOpenSubCategoryService>().SingleInstance();
                containerBuilder.RegisterType<PicturesService>().As<IPicturesService>().SingleInstance();
                containerBuilder.RegisterType<QuestionsService>().As<IQuestionsService>().SingleInstance();
                containerBuilder.RegisterType<ScoresService>().As<IScoresService>().SingleInstance();
                containerBuilder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();
                containerBuilder.RegisterType<SignalRServiceBase>().As<ISignalRServiceBase>().SingleInstance();
                containerBuilder.RegisterType<SQLiteService<UserModel>>().As<ISQLiteService<UserModel>>().SingleInstance();
                containerBuilder.RegisterType<SQLiteService<LanguageModel>>().As<ISQLiteService<LanguageModel>>().SingleInstance();
                containerBuilder.RegisterType<SubCategoriesService>().As<ISubCategoriesService>().SingleInstance();
                containerBuilder.RegisterType<SupportService>().As<ISupportService>().SingleInstance();
                containerBuilder.RegisterType<UserDataModule>().As<IUserDataModule>().SingleInstance();
                containerBuilder.RegisterType<PostsService>().As<IPostsService>().SingleInstance();
                containerBuilder.RegisterType<DuelModule>().As<IDuelModule>().SingleInstance();*/
        }

        #endregion Register Instance
    }
}