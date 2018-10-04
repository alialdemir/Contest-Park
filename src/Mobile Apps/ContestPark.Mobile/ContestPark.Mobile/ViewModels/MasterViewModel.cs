using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class MasterViewModel : ViewModelBase<MenuItemList>
    {
        #region Private variables

        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructors

        public MasterViewModel(INavigationService navigationService,
                               IEventAggregator eventAggregator,
                               ISettingsService settingsService) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            FullName = settingsService.CurrentUser.FullName;
            CoverPicture = settingsService.CurrentUser.CoverPicturePath;
            InitializeAsync();
        }

        #endregion Constructors

        #region Property

        private string _coverPicture;

        public string CoverPicture
        {
            get { return _coverPicture; }
            set
            {
                _coverPicture = value;

                RaisePropertyChanged(() => CoverPicture);
            }
        }

        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;

                RaisePropertyChanged(() => FullName);
            }
        }

        #endregion Property

        #region Methods

        protected override Task InitializeAsync()
        {
            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList("Menu")
                            {
                                new Models.MenuItem.MenuItem {
                                    PageName = nameof(ContestStoreView),
                                    Icon = "ic_shopping_cart_black_24dp.png",
                                    Title = ContestParkResources.ContestStore,
                                    MenuType = Enums.MenuTypes.Icon
                                },
                                new Models.MenuItem.MenuItem {
                                    PageName = nameof(MissionsView),
                                    Icon = "ic_filter_list_black_24dp.png",
                                    Title = ContestParkResources.Missions,
                                    MenuType = Enums.MenuTypes.Icon
                                },
                                new Models.MenuItem.MenuItem {
                                    PageName = nameof(SettingsView),
                                    Icon = "ic_settings_applications_black_24dp.png",
                                    Title = ContestParkResources.Settings,
                                    MenuType = Enums.MenuTypes.Icon
                                },
                            },
                new MenuItemList(ContestParkResources.FollowUsOnSocialNetworks)
                            {
                                 new Models.MenuItem.MenuItem {
                                     PageName = "FacebookAddress",
                                     Icon = "facebook_36x.png",
                                     Title = "Facebook",
                                     MenuType = Enums.MenuTypes.Icon
                                 },
                                 new Models.MenuItem.MenuItem {
                                     PageName = "TwitterAddress",
                                     Icon = "twitter_36x.png",
                                     Title = "Twitter",
                                     MenuType = Enums.MenuTypes.Icon
                                 },
                                 new Models.MenuItem.MenuItem {
                                     PageName = "InstagramAddress",
                                     Icon = "instagram_36x.png",
                                     Title = "Instagram",
                                     MenuType = Enums.MenuTypes.Icon
                                 }
                            }
            });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        /// <param name="pageName">Yönlendirilecek sayfa adı</param>
        private void ExecutePushPageCommand(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName))
            {
                _eventAggregator
                .GetEvent<MasterDetailPageIsPresentedEvent>()
                .Publish(false);
                _eventAggregator
                        .GetEvent<TabPageNavigationEvent>()
                        .Publish(pageName);
            }
        }

        #endregion Methods

        #region Commands

        private ICommand _pushPageCommand;

        public ICommand PushPageCommand
        {
            get { return _pushPageCommand ?? (_pushPageCommand = new Command<string>((pageName) => ExecutePushPageCommand(pageName))); }
        }

        #endregion Commands
    }
}