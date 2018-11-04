using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Services.Cp;
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

        private readonly ICpService _cpService;

        #endregion Private variables

        #region Constructors

        public MasterViewModel(INavigationService navigationService,
                               IEventAggregator eventAggregator,
                               ICpService cpService,
                               ISettingsService settingsService) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _cpService = cpService;

            FullName = settingsService.CurrentUser.FullName;
            CoverPicture = settingsService.CurrentUser.CoverPicturePath;
            ProfilePicture = settingsService.CurrentUser.ProfilePicturePath;

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

        private string _profilePicture;

        public string ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                _profilePicture = value;

                RaisePropertyChanged(() => ProfilePicture);
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

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        private string _userCoins = "0";

        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public string UserCoins
        {
            get
            {
                return _userCoins;
            }
            set
            {
                _userCoins = value;
                RaisePropertyChanged(() => UserCoins);
            }
        }

        #endregion Property

        #region Methods

        protected override Task InitializeAsync()
        {
            SetUserGoldCommand.Execute(null);

            ICommand pushPageCommand = new Command<string>((pageName) => ExecutePushPageCommand(pageName));

            Items.AddRange(new List<MenuItemList>()
            {
                new MenuItemList("Menu")
                            {
                                new Models.MenuItem.MenuItem {
                                    CommandParameter = nameof(ContestStoreView),
                                    Icon = "fas-store",
                                    Title = ContestParkResources.ContestStore,
                                    MenuType = Enums.MenuTypes.Icon,
                                    SingleTap = pushPageCommand
                                },
                                new Models.MenuItem.MenuItem {
                                    CommandParameter = nameof(MissionsView),
                                    Icon = "fas-award",
                                    Title = ContestParkResources.Missions,
                                    MenuType = Enums.MenuTypes.Icon,
                                    SingleTap = pushPageCommand
                                },
                                new Models.MenuItem.MenuItem {
                                    CommandParameter = nameof(SettingsView),
                                    Icon = "fas-cogs",
                                    Title = ContestParkResources.Settings,
                                    MenuType = Enums.MenuTypes.Icon,
                                    SingleTap = pushPageCommand
                                },
                            },
                new MenuItemList(ContestParkResources.FollowUsOnSocialNetworks)
                            {
                                 new Models.MenuItem.MenuItem {
                                     CommandParameter = "FacebookAddress",
                                     Icon = "fab-facebook-square",
                                     Title = "Facebook",
                                     MenuType = Enums.MenuTypes.Icon,
                                        SingleTap = pushPageCommand
                                 },
                                 new Models.MenuItem.MenuItem {
                                     CommandParameter = "TwitterAddress",
                                     Icon = "fab-twitter-square",
                                     Title = "Twitter",
                                     MenuType = Enums.MenuTypes.Icon,
                                        SingleTap = pushPageCommand
                                 },
                                 new Models.MenuItem.MenuItem {
                                     CommandParameter = "InstagramAddress",
                                     Icon = "fab-instagram",
                                     Title = "Instagram",
                                     MenuType = Enums.MenuTypes.Icon,
                                     SingleTap = pushPageCommand
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
            if (pageName.Equals("FacebookAddress"))
            {
            }
            else if (pageName.Equals("TwitterAddress"))
            {
            }
            else if (pageName.Equals("InstagramAddress"))
            {
            }
            else if (!string.IsNullOrEmpty(pageName))
            {
                _eventAggregator
                .GetEvent<MasterDetailPageIsPresentedEvent>()
                .Publish(false);
                _eventAggregator
                        .GetEvent<TabPageNavigationEvent>()
                        .Publish(pageName);
            }
        }

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        /// <returns></returns>
        private async Task SetUserGoldAsync()
        {
            int userGold = await _cpService.GetTotalCpByUserIdAsync();
            if (userGold > 0) UserCoins = userGold.ToString("##,#").Replace(",", ".");
            else UserCoins = userGold.ToString();
        }

        #endregion Methods

        #region Commands

        private ICommand SetUserGoldCommand => new Command(async () => await SetUserGoldAsync());

        #endregion Commands
    }
}