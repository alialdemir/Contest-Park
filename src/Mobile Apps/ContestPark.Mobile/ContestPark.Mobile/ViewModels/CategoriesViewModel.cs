using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Events;
using ContestPark.Mobile.Extensions;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Balance;
using ContestPark.Mobile.Models.Categories;
using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Slide;
using ContestPark.Mobile.Models.Token;
using ContestPark.Mobile.Services.AdMob;
using ContestPark.Mobile.Services.Analytics;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Cp;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Identity;
using ContestPark.Mobile.Services.InviteDuel;
using ContestPark.Mobile.Services.Notice;
using ContestPark.Mobile.Services.Notification;
using ContestPark.Mobile.Services.Settings;
using ContestPark.Mobile.Services.Signalr.Base;
using ContestPark.Mobile.Services.Signalr.Duel;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Microsoft.AppCenter.Crashes;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;

//using Shiny.Push;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategoriesViewModel : ViewModelBase<CategoryModel>
    {
        #region Private variables

        private readonly ISignalRServiceBase _baseSignalRService;
        private readonly INotificationService _notificationService;
        private readonly IGameService _gameService;
        private readonly INoticeService _noticeService;
        private readonly IAdMobService _adMobService;
        private readonly IBalanceService _balanceService;
        private readonly IAnalyticsService _analyticsService;

        //private readonly IPushManager _pushManager;
        private readonly IInviteDuelService _inviteDuelService;

        private readonly IIdentityService _identityService;
        private readonly ISettingsService _settingsService;
        private readonly IDuelSignalRService _duelSignalRService;
        private readonly ICategoryService _categoryServices;

        #endregion Private variables

        #region Constructor

        public CategoriesViewModel(ICategoryService categoryServices,
                                   ISignalRServiceBase baseSignalRService,// signalr bağlantısı başlatılması için ekledim
                                   INavigationService navigationService,
                                   IPageDialogService pageDialogService,
                                   INotificationService notificationService,
                                   IGameService gameService,
                                   INoticeService noticeService,
                                   IAdMobService adMobService,
                                   IBalanceService balanceService,
                                   IAnalyticsService analyticsService,
                                   // IPushManager pushManager,
                                   IInviteDuelService inviteDuelService,
                                   IIdentityService identityService,
                                   ISettingsService settingsService,
                                   IDuelSignalRService duelSignalRService,
                                   IEventAggregator eventAggregator
            ) : base(navigationService, pageDialogService)
        {
            _categoryServices = categoryServices;

            gameService.NavigationService = navigationService;

            EventSubscribe(eventAggregator);
            ServiceModel.PageSize = 9999;// Şimdilik 9999 verdim kategorilerde safyalama yok

            _baseSignalRService = baseSignalRService;
            _notificationService = notificationService;
            _gameService = gameService;
            _noticeService = noticeService;
            _adMobService = adMobService;
            _balanceService = balanceService;
            _analyticsService = analyticsService;
            //   _pushManager = pushManager;
            _inviteDuelService = inviteDuelService;
            _identityService = identityService;
            _settingsService = settingsService;
            _duelSignalRService = duelSignalRService;

            #region Skeleton loading

            var categories = new CategoryModel
            {
                IsBusy = true,
                SubCategories = new System.Collections.Generic.List<SubCategoryModel>
                {
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                  new SubCategoryModel{ IsBusy = true, DisplayPrice = "0" },
                }
            };

            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);
            Items.Add(categories);

            #endregion Skeleton loading
        }

        #endregion Constructor

        #region Properties

        public short SeeAllSubCateogryId { get; set; } = 0;

        private IEnumerable<NoticeModel> _notices;

        public IEnumerable<NoticeModel> Notices
        {
            get { return _notices; }
            set
            {
                _notices = value;

                RaisePropertyChanged(() => Notices);
            }
        }

        #endregion Properties

        #region Methods

        public override async Task InitializeAsync(INavigationParameters parameters = null)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (!IsRefreshing)
            {
                ConnectToSignalr.Execute(null);

                CheckRewardCommand.Execute(null);

                ScopeRefleshCommand.Execute(null);

                ListenerFirebaseToken.Execute(null);

                NoticeCommand.Execute(null);
#if !DEBUG
                _inviteDuelService.InviteDuelCommand.Execute(Items);
#endif
            }

            //TODO: Kategorileri sayfala
            ServiceModel = await _categoryServices.CategoryListAsync(ServiceModel, IsRefreshing);

            await base.InitializeAsync(parameters);

            IsBusy = false;
        }

        /// <summary>
        /// Event dinleme
        /// </summary>
        private void EventSubscribe(IEventAggregator eventAggregator) => eventAggregator.GetEvent<SubCategoryRefleshEvent>()
                .Subscribe(() => RefreshCommand.Execute(null));

        /// <summary>
        /// Kategori search sayfasına gider
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private void ExecutGoToCategorySearchPageCommand(short categoryId = 0)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (categoryId != 0)
            {
                string subCategoryName = Items.FirstOrDefault(x => x.CategoryId == categoryId).CategoryName;

                _analyticsService.SendEvent("Kategori", "Tümünü Gör", subCategoryName);
            }

            NavigateToAsync<SearchView>(new NavigationParameters
                                                {
                                                    { "CategoryId", categoryId }
                                                });

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategoriye uzun basınca ActionSheet gösterir
        /// </summary>
        private void AddLongPressed(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            _gameService?.SubCategoriesDisplayActionSheetAsync(new SelectedSubCategoryModel
            {
                SubcategoryId = subCategory.SubCategoryId,
                SubCategoryName = subCategory.SubCategoryName,
                SubCategoryPicturePath = subCategory.PicturePath
            }, subCategory.IsSubCategoryOpen);

            IsBusy = false;
        }

        /// <summary>
        /// Alt kategoriye tıklanınca kategori detaya gider
        /// </summary>
        private void AddSingleTap(SubCategoryModel subCategory)
        {
            if (IsBusy || subCategory == null)
                return;

            IsBusy = true;

            _gameService?.PushCategoryDetailViewAsync(subCategory.SubCategoryId,
                                                      subCategory.IsSubCategoryOpen,
                                                      subCategory.SubCategoryName);

            IsBusy = false;
        }

        /// <summary>
        /// Düello davetleri buraya düşer
        /// </summary>
        private void OnInviteModel(object sender, InviteModel e)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            string popupName = CurrentPopupName();
            if (popupName != nameof(DuelStartingPopupView)
                && popupName != nameof(QuestionPopupView)
                && popupName != nameof(QuestionExpectedPopupView)
                && popupName != nameof(AcceptDuelInvitationPopupView))
            {
                NavigateToPopupAsync<AcceptDuelInvitationPopupView>(new NavigationParameters
                {
                    { "InviteModel", (InviteModel)sender }
                });
            }

            IsBusy = false;
        }

        /// <summary>
        /// Duyuruları getirir
        /// </summary>
        private async Task ExecuteNoticeCommand()
        {
            var notices = await _noticeService.NoticesAsync(new PagingModel());
            Notices = notices.Items;
        }

        /// <summary>
        /// Kategorilerin üst kısmındaki duyuru resimlerine tıklayınca sayfa yönlendirmesi yapar
        /// </summary>
        /// <param name="link">Web sitesi linki veya view linki</param>
        private void ExecuteNoticeNavigateToCommand(string link)
        {
            if (IsBusy || string.IsNullOrEmpty(link))
                return;

            IsBusy = true;

            try
            {
                if (link.IsUrl())
                {
                    NavigateToAsync<BrowserView>(new NavigationParameters
                                                        {
                                                            { "Link", link }
                                                        });
                }
                else
                    NavigateToAsync(link);

                _analyticsService.SendEvent("Kategori", "Slider tıklaması", link);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand NoticeCommand
        {
            get
            {
                return new CommandAsync(ExecuteNoticeCommand);
            }
        }

        private ICommand _noticeNavigateToCommand;

        public ICommand NoticeNavigateToCommand
        {
            get
            {
                return _noticeNavigateToCommand ?? (_noticeNavigateToCommand = new Command<string>(ExecuteNoticeNavigateToCommand));
            }
        }

        /// <summary>
        /// Firebase token değişince sunucuya bildirir
        /// </summary>
        public ICommand ListenerFirebaseToken
        {
            get
            {
                return new Command(() =>
               {
                   try
                   {
                       //PushAccessState token = await _pushManager.RequestAccess();
                       //if (token.Status == Shiny.AccessState.Available && !string.IsNullOrEmpty(token.RegistrationToken))
                       //{
                       //    _notificationService?.UpdatePushTokenAsync(new PushNotificationTokenModel
                       //    {
                       //        Token = token.RegistrationToken
                       //    });
                       //}
                   }
                   catch (Exception ex)
                   {
                       Debug.WriteLine(ex.Message);
                   }
               });
            }
        }

        private ICommand _goToCategorySearchPageCommand;
        public ICommand GoToCategorySearchPageCommand => _goToCategorySearchPageCommand ?? (_goToCategorySearchPageCommand = new Command<short>(ExecutGoToCategorySearchPageCommand));

        public ICommand GotoNotificationsCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (IsBusy)
                        return;

                    IsBusy = true;

                    NavigateToAsync<NotificationView>();

                    IsBusy = false;
                });
            }
        }

        public ICommand InviteCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (IsBusy)
                        return;

                    IsBusy = true;

                    if (_settingsService.CurrentUser.UserId != "34873f81-dfee-4d78-bc17-97d9b9bb-bot")
                        NavigateToPopupAsync<InviteView>();
                    else
                        _gameService.SubCategoryShare();

                    IsBusy = false;
                });
            }
        }

        private ICommand ConnectToSignalr => new Command(() =>
       {
           Device.BeginInvokeOnMainThread(async () =>
           {
               if (!_baseSignalRService.IsConnect)
                   await _baseSignalRService.Init();

               // Düello daveti dinleme
               _duelSignalRService.InviteDuelEventHandler += OnInviteModel;
               _duelSignalRService.InviteDuel();
           });
       });

        private ICommand _pushCategoryDetailViewCommand;

        public ICommand PushCategoryDetailViewCommand
        {
            get
            {
                return _pushCategoryDetailViewCommand ?? (_pushCategoryDetailViewCommand = new Command<SubCategoryModel>(AddSingleTap));
            }
        }

        private ICommand _subCategoriesDisplayActionSheetCommand;

        /// <summary>
        /// Alt kategori display alert command
        /// </summary>
        public ICommand SubCategoriesDisplayActionSheetCommand
        {
            get
            {
                return _subCategoriesDisplayActionSheetCommand ?? (_subCategoriesDisplayActionSheetCommand = new Command<SubCategoryModel>(AddLongPressed));
            }
        }

        /// <summary>
        /// Hediye altın kazanma durumunu kontrol edip kazanmışsa popup açar
        /// </summary>
        public ICommand CheckRewardCommand
        {
            get
            {
                return new Command(async () =>
                {
                    RewardModel giftGold = await _balanceService.RewardAsync();
                    if (giftGold.Amount > 0)
                    {
                        await NavigateToPopupAsync<GiftGoldPopupView>(new NavigationParameters
                        {
                            { "RewardModel", giftGold }
                        });
                    }
                });
            }
        }

        /// <summary>
        /// Token alırken scope kısmında gönderdiğimiz servislere yenisi eklenirse o servisi kullanmak için tekrar token alıp set ediyoruz
        /// </summary>
        public ICommand ScopeRefleshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (_settingsService.LastUpdatedScopeName != GlobalSetting.Scope)
                    {
                        string phoneNumber = await _identityService.GetPhoneNumber();
                        if (string.IsNullOrEmpty(phoneNumber))
                            return;

                        UserToken token = await _identityService.GetTokenAsync(new LoginModel
                        {
                            Password = phoneNumber,
                            UserName = _settingsService.CurrentUser.UserName
                        });
                        if (token != null)
                        {
                            _settingsService.SetTokenInfo(token);

                            _settingsService.LastUpdatedScopeName = GlobalSetting.Scope;
                        }
                    }
                });
            }
        }

        #endregion Commands
    }
}
