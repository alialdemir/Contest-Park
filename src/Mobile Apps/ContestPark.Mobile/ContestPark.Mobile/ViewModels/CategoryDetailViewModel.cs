using ContestPark.Mobile.Events;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Services.Category;
using ContestPark.Mobile.Services.Game;
using ContestPark.Mobile.Services.Post;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Events;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class CategoryDetailViewModel : ViewModelBase<PostModel>
    {
        #region Private variable

        private readonly ICategoryServices _categoryServices;
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameService _gameService;
        private readonly IPostService _postService;
        private short _subCategoryId = 0;

        #endregion Private variable

        #region Constructor

        public CategoryDetailViewModel(INavigationService navigationService,
                                       IPopupNavigation popupNavigation,
                                       ICategoryServices categoryServices,
                                       IPostService postService,
                                       IGameService gameService,
                                       IEventAggregator eventAggregator) : base(navigationService, popupNavigation: popupNavigation)
        {
            NavigationService = navigationService;
            _categoryServices = categoryServices;
            _postService = postService;
            _gameService = gameService;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// İlgili kategoriyi takip eden kişi sayısı
        /// </summary>
        private int _followersCount = 0;

        /// <summary>
        /// Alt kategori takip etme durumu
        /// </summary>
        private bool _isSubCategoryFollowUpStatus;

        private int _level = 1;

        private string _subCategoryPicturePath;

        public int FollowersCount
        {
            get { return _followersCount; }
            set
            {
                _followersCount = value;
                RaisePropertyChanged(() => FollowersCount);
            }
        }

        public bool IsSubCategoryFollowUpStatus
        {
            get { return _isSubCategoryFollowUpStatus; }
            set
            {
                _isSubCategoryFollowUpStatus = value;
                RaisePropertyChanged(() => IsSubCategoryFollowUpStatus);
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                RaisePropertyChanged(() => Level);
            }
        }

        public string SubCategoryPicturePath
        {
            get
            {
                return _subCategoryPicturePath;
            }
            set
            {
                _subCategoryPicturePath = value;
                RaisePropertyChanged(() => SubCategoryPicturePath);
            }
        }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            SSubCategoryPostsCommand.Execute(null);

            IsFollowUpStatusCommand.Execute(null);

            FollowersCountCommand.Execute(null);

            await base.InitializeAsync();

            IsBusy = false;
        }

        /// <summary>
        /// Düello bahis panelini aç
        /// </summary>
        private async Task ExecuteduelOpenPanelCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await PushPopupPageAsync(new DuelBettingPopupView()
            {
                SubcategoryId = _subCategoryId,
                SubcategoryName = Title,
                SubCategoryPicturePath = SubCategoryPicturePath,
            });

            IsBusy = false;
        }

        /// <summary>
        /// Kategori takip eden kullanıcı sayısı
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteFollowersCountCommandAsync()
        {
            FollowersCount = await _categoryServices?.FollowersCountAsync(_subCategoryId);
        }

        /// <summary>
        /// Sıralama sayfasına git
        /// </summary>
        private async Task ExecuteGoToRankingPageCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(RankingView), new NavigationParameters
            {
                {"SubCategoryId", _subCategoryId },
                {"SubCategoryName", Title },
                {"ListType", RankingViewModel.ListTypes.ScoreRanking },
            });

            IsBusy = false;
        }

        /// <summary>
        /// İlgili kategoriyi takip etme durumu
        /// </summary>
        private async Task ExecuteIsFollowUpStatusCommandAsync()
        {
            IsSubCategoryFollowUpStatus = await _categoryServices?.IsFollowUpStatusAsync(_subCategoryId);
        }

        /// <summary>
        /// Alt kategori takip et takibi bırak methodu takip ediyorsa takibi bırakır takip etmiyorsa takip eder
        /// </summary>
        private async Task ExecuteSubCategoryFollowProgcessCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            FollowCountChange();

            bool isSuccess = await _categoryServices.SubCategoryFollowProgcess(_subCategoryId, IsSubCategoryFollowUpStatus);
            if (isSuccess)
            {
                _eventAggregator
                            .GetEvent<SubCategoryRefleshEvent>()
                            .Publish();
            }
            else FollowCountChange();// eğer işlem başarısız ise takibi geri aldık

            IsBusy = false;
        }

        /// <summary>
        /// Kategori takipçi sayısını artırıp azaltır ve takip etme durumunu değiştirir
        /// </summary>
        private void FollowCountChange()
        {
            IsSubCategoryFollowUpStatus = !IsSubCategoryFollowUpStatus;

            if (IsSubCategoryFollowUpStatus) FollowersCount++;
            else FollowersCount--;
        }

        #endregion Methods

        #region Commands

        private ICommand duelOpenPanelCommand;

        private ICommand followersCountCommand;

        private ICommand goToRankingPageCommand;

        private ICommand isFollowUpStatusCommand;

        private ICommand shareCommand;

        private ICommand subCategoryFollowProgcessCommand;

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public ICommand DuelOpenPanelCommand
        {
            get { return duelOpenPanelCommand ?? (duelOpenPanelCommand = new Command(async () => await ExecuteduelOpenPanelCommandAsync())); }
        }

        /// <summary>
        /// Alt kategori takipçi sayısı command
        /// </summary>
        public ICommand FollowersCountCommand
        {
            get { return followersCountCommand ?? (followersCountCommand = new Command(async () => await ExecuteFollowersCountCommandAsync())); }
        }

        /// <summary>
        /// Sıralama sayfasına git command
        /// </summary>
        public ICommand GoToRankingPageCommand
        {
            get { return goToRankingPageCommand ?? (goToRankingPageCommand = new Command(async () => await ExecuteGoToRankingPageCommandAsync())); }
        }

        /// <summary>
        /// Takip etme durmunu çalıştır command
        /// </summary>
        public ICommand IsFollowUpStatusCommand
        {
            get { return isFollowUpStatusCommand ?? (isFollowUpStatusCommand = new Command(async () => await ExecuteIsFollowUpStatusCommandAsync())); }
        }

        /// <summary>
        /// Düello bahis paneli aç command
        /// </summary>
        public INavigationService NavigationService { get; }

        /// <summary>
        /// Sosyal ağda paylaş command
        /// </summary>
        public ICommand ShareCommand
        {
            get { return shareCommand ?? (shareCommand = new Command(() => _gameService?.SubCategoryShare(Title))); }
        }

        /// <summary>
        /// Alt kategori takip etme veya takip bırakma command
        /// </summary>
        public ICommand SubCategoryFollowProgcessCommand
        {
            get { return subCategoryFollowProgcessCommand ?? (subCategoryFollowProgcessCommand = new Command(async () => await ExecuteSubCategoryFollowProgcessCommandAsync())); }
        }

        private ICommand SSubCategoryPostsCommand
        {
            get
            {
                return new Command(async () => ServiceModel = await _postService.SubCategoryPostsAsync(_subCategoryId, ServiceModel));
            }
        }

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryName")) Title = parameters.GetValue<string>("SubCategoryName");
            if (parameters.ContainsKey("SubCategoryPicturePath")) SubCategoryPicturePath = parameters.GetValue<string>("SubCategoryPicturePath");
            if (parameters.ContainsKey("SubCategoryId")) _subCategoryId = parameters.GetValue<Int16>("SubCategoryId");

            base.OnNavigatingTo(parameters);
        }

        #endregion Navigation
    }
}