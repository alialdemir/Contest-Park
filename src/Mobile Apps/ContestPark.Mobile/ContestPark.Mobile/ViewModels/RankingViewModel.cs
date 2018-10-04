using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Services.Score;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ContestPark.Mobile.ViewModels
{
    public class RankingViewModel : ViewModelBase<RankingModel>
    {
        #region Enum

        public enum ListTypes
        {
            ScoreRanking = 0,
            ScoreRankingFollowing = 1
        }

        #endregion Enum

        #region Private variables

        private readonly IScoreService _scoreService;

        #endregion Private variables

        #region Constructors

        public RankingViewModel(INavigationService navigationService,
                                IScoreService scoreService) : base(navigationService)
        {
            _scoreService = scoreService;
        }

        #endregion Constructors

        #region Properties

        private short SubCategoryId { get; set; }

        public ListTypes ListType { get; set; }

        private string _rankEmptyMessage;

        public string RankEmptyMessage
        {
            get { return _rankEmptyMessage; }
            set
            {
                _rankEmptyMessage = value;
                RaisePropertyChanged(() => RankEmptyMessage);
            }
        }

        private TimeLeftModel _timeLeftModel;

        public TimeLeftModel TimeLeft
        {
            get { return _timeLeftModel; }
            set
            {
                _timeLeftModel = value;
                RaisePropertyChanged(() => TimeLeft);
            }
        }

        /// <summary>
        /// Sayfadan çıkınca timer durduruldu
        /// </summary>
        public bool IsTimerStop { get; set; } = true;

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            if (ListTypes.ScoreRanking == ListType)
            {
                ServiceModel = await _scoreService.SubCategoryRankingAsync(SubCategoryId, ServiceModel);
            }
            else if (ListTypes.ScoreRankingFollowing == ListType)
            {
                ServiceModel = await _scoreService.FollowingRankingAsync(SubCategoryId, ServiceModel);
            }

            await base.InitializeAsync();

            if (TimeLeft == null)
            {
                TimeLeftCommand.Execute(null);
            }

            LoadRankEmptyMessage();

            IsBusy = false;
        }

        /// <summary>
        /// ListTypes göre listview boş olduğunda çıkacak mesajı ayarlar
        /// </summary>
        private void LoadRankEmptyMessage()
        {
            switch (ListType)
            {
                case ListTypes.ScoreRankingFollowing: RankEmptyMessage = ContestParkResources.RankFollowingNull; break;
                default: RankEmptyMessage = ContestParkResources.ThisCategoryRankNull; break;
            }
        }

        /// <summary>
        /// Segmente tıklanınca listeleme tipi değiştir
        /// </summary>
        /// <param name="value">seçilen segment id</param>
        private void SegmentValueChanged(int selectedSegmentIndex)
        {
            if (!IsInitialized)
                return;

            switch (selectedSegmentIndex)
            {
                case 1: ListType = ListTypes.ScoreRankingFollowing; break;
                default: ListType = ListTypes.ScoreRanking; break;
            }

            LoadRankEmptyMessage();

            RefleshCommand.Execute(null);
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });
            }
        }

        /// <summary>
        /// Yarışmanın biteceği zamanı verir
        /// </summary>
        private async Task ExecuteTimeLeftCommand()
        {
            TimeLeft = await _scoreService.GetTimeLeft(SubCategoryId);

            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                if (!IsTimerStop)
                    return IsTimerStop;

                TimeSpan diff = TimeLeft.FinsihDate - DateTime.Now;// Datetime.now telefonun tarihi yanlışsa yanlış tarih gösterir

                TimeLeft.TimeLeft = diff.Days +
                                        ContestParkResources.ShortDay +
                                        diff.Hours + ContestParkResources.ShortHour +
                                        diff.Minutes + ContestParkResources.ShortMinute +
                                        diff.Seconds + ContestParkResources.ShortSeconds;

                return !(diff.Days == 0 && diff.Hours == 0 && diff.Minutes == 0 && diff.Seconds == 0);
            });
        }

        #endregion Methods

        #region Commands

        private ICommand TimeLeftCommand => new Command(async () => await ExecuteTimeLeftCommand());

        public ICommand SegmentValueChangedCommand => new Command<int>((selectedSegmentIndex) => SegmentValueChanged(selectedSegmentIndex));

        private ICommand _gotoProfilePageCommand;

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)));

        #endregion Commands

        #region Navigation

        public override void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryId")) SubCategoryId = parameters.GetValue<short>("SubCategoryId");
            if (parameters.ContainsKey("SubCategoryName")) Title = parameters.GetValue<string>("SubCategoryName");
            if (parameters.ContainsKey("ListType")) ListType = parameters.GetValue<ListTypes>("ListType");

            base.OnNavigatingTo(parameters);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsTimerStop = false;

            base.OnNavigatedFrom(parameters);
        }

        #endregion Navigation
    }
}