using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.User;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Events;
using Prism.Navigation;

namespace ContestPark.Mobile.ViewModels
{
    public class TabViewModel : ViewModelBase
    {
        #region Constructors

        public TabViewModel(INavigationService navigationService,
                            ISettingsService settingsService,
                            IEventAggregator eventAggregator) : base(navigationService)
        {
            CurrentUser = settingsService.CurrentUser;

            eventAggregator
                        .GetEvent<TabPageNavigationEvent>()
                        .Subscribe((page) => PushNavigationPageAsync(page.PageName, page.Parameters));
        }

        #endregion Constructors

        #region Property

        public UserInfoModel CurrentUser { get; private set; }

        #endregion Property

        #region Methods

        /// <summary>
        /// Mesajlar ve bildirimler de görünmeyen bildirimleri kontrol eder ve kendi profili için parametre verir
        /// </summary>
        private void TabsBindingContext()
        {
            //tabCategoriesPage.BindingContext = new CategoriesPageViewModel();
            //tabProfilePage.BindingContext = new ProfilePageViewModel(UserDataModule.UserModel.UserName) { IsVisibleBackArrow = false };

            //NotificationsPageViewModel notificationViewModel = new NotificationsPageViewModel();
            //tabNotificationsPage.BindingContext = notificationViewModel;
            //((NotificationsPageViewModel)tabNotificationsPage.BindingContext).UserNotificationVisibilityCountCommand.Execute(null);

            //((ChatAllPageViewModel)tabChatAllPage.BindingContext).UserChatVisibilityCountCommand.Execute(null);
        }

        #endregion Methods
    }
}