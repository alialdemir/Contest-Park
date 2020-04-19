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

            containerRegistry.RegisterPopupNavigationService();

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
    }
}
