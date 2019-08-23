using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.MenuItem;
using ContestPark.Mobile.Models.PageNavigation;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class MasterViewModel : ViewModelBase<MenuItemList>
    {
        #region Private variables

        private readonly IBalanceService _cpService;
        private readonly IEventAggregator _eventAggregator;

        #endregion Private variables

        #region Constructors

        public MasterViewModel(INavigationService navigationService,
                               IEventAggregator eventAggregator,
                               IBalanceService cpService,
                               ISettingsService settingsService) : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _cpService = cpService;

            FullName = settingsService.CurrentUser.FullName;
            ProfilePicture = settingsService.CurrentUser.ProfilePicturePath;

            InitializeAsync();
        }

        #endregion Constructors

        #region Property

        private string _fullName;

        private string _profilePicture;

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        private BalanceModel _balance = new BalanceModel();

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;

                RaisePropertyChanged(() => FullName);
            }
        }

        public string ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                _profilePicture = value;

                RaisePropertyChanged(() => ProfilePicture);
            }
        }

        /// <summary>
        /// Public property to set and get the title of the item
        /// </summary>
        public BalanceModel Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
                RaisePropertyChanged(() => Balance);
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
                                new TextMenuItem {
                                    CommandParameter = nameof(ContestStoreView),
                                    Icon = "conteststore.svg",
                                    Title = ContestParkResources.ContestStore,
                                    MenuType = Enums.MenuTypes.Label,
                                    SingleTap = pushPageCommand
                                },
                                new TextMenuItem {
                                    CommandParameter = nameof(MissionsView),
                                    Icon = "missions.svg",
                                    Title = ContestParkResources.Missions,
                                    MenuType = Enums.MenuTypes.Label,
                                    SingleTap = pushPageCommand
                                },
                                new TextMenuItem {
                                    CommandParameter = nameof(SettingsView),
                                    Icon = "settings.svg",
                                    Title = ContestParkResources.Settings,
                                    MenuType = Enums.MenuTypes.Label,
                                    SingleTap = pushPageCommand
                                },
                            },
                new MenuItemList(ContestParkResources.FollowUsOnSocialNetworks)
                            {
                                 new TextMenuItem {
                                     CommandParameter = "facebook",
                                     Icon = "facebook.svg",
                                     Title = "Facebook",
                                     MenuType = Enums.MenuTypes.Label,
                                        SingleTap = pushPageCommand
                                 },
                                 new TextMenuItem {
                                     CommandParameter = "twitter",
                                     Icon = "twitter.svg",
                                     Title = "Twitter",
                                     MenuType = Enums.MenuTypes.Label,
                                        SingleTap = pushPageCommand
                                 },
                                 new TextMenuItem {
                                     CommandParameter = "instagram",
                                     Icon = "instagram.svg",
                                     Title = "Instagram",
                                     MenuType = Enums.MenuTypes.Label,
                                     SingleTap = pushPageCommand
                                 }
                            }
            });

            _eventAggregator
                .GetEvent<ChangeUserInfoEvent>()
                .Subscribe((userInfo) =>
                {
                    if (userInfo != null)
                    {
                        FullName = userInfo.FullName;
                        ProfilePicture = userInfo.ProfilePicturePath;
                    }
                });

            return base.InitializeAsync();
        }

        /// <summary>
        /// Parametreden gelen view adına göre yönlendirme yapar ve sol menuyu kapatır
        /// </summary>
        /// <param name="pageName">Yönlendirilecek sayfa adı</param>
        private void ExecutePushPageCommand(string name)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (name == "facebook" || name == "twitter" || name == "instagram")
            {
                OpenUri($"https://www.{name}.com/contestpark");
            }
            else if (!string.IsNullOrEmpty(name))
            {
                _eventAggregator
                        .GetEvent<MasterDetailPageIsPresentedEvent>()
                        .Publish(false);

                _eventAggregator
                        .GetEvent<TabPageNavigationEvent>()
                        .Publish(new PageNavigation(name));
            }

            IsBusy = false;
        }

        /// <summary>
        /// gelen linki browser da açar
        /// </summary>
        /// <param name="url">web site link</param>
        private void OpenUri(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Device.OpenUri(new Uri(url));
            }
        }

        /// <summary>
        /// Kullanıcı altın miktarı
        /// </summary>
        /// <returns></returns>
        private async Task SetUserGoldAsync()
        {
            Balance = await _cpService.GetTotalCpByUserIdAsync();
            //BalanceGold = balance.Gold.ToString();

            //BalanceMoney = balance.Money > 0 ?
            //    balance.Money.ToString("##,#").Replace(",", ".") :
            //    balance.Money.ToString();
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// Profil sayfasına yönlendirir
        /// </summary>
        public ICommand GotoProfileViewCommand
        {
            get => new Command(() =>
            {
                if (_eventAggregator != null)
                {
                    _eventAggregator.GetEvent<TabChangeEvent>().Publish(Enums.Tabs.Profile);

                    _eventAggregator
                                .GetEvent<MasterDetailPageIsPresentedEvent>()
                                .Publish(false);
                }
            });
        }

        private ICommand SetUserGoldCommand => new Command(async () => await SetUserGoldAsync());

        #endregion Commands
    }
}
