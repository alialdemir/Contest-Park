using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Services.Score;
using ContestPark.Mobile.ViewModels.Base;
using ContestPark.Mobile.Views;
using Prism.Navigation;
using System;
using System.Diagnostics;
using System.Linq;
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
            Title = ContestParkResources.Ranking;
        }

        #endregion Constructors

        #region Properties

        private string _rankEmptyMessage;
        private RankModel _timeLeftModel;

        /// <summary>
        /// Sayfadan çıkınca timer durduruldu
        /// </summary>
        public bool IsTimerStop { get; set; } = true;

        public ListTypes ListType { get; set; }

        public string RankEmptyMessage
        {
            get { return _rankEmptyMessage; }
            set
            {
                _rankEmptyMessage = value;
                RaisePropertyChanged(() => RankEmptyMessage);
            }
        }

        public RankModel Ranks
        {
            get { return _timeLeftModel; }
            set
            {
                _timeLeftModel = value;
                RaisePropertyChanged(() => Ranks);
            }
        }

        private short SubCategoryId { get; set; }

        #endregion Properties

        #region Methods

        protected override async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            BalanceTypes balanceType = BalanceTypes.Gold;

            RankModel rank = null;

            switch (ListType)
            {
                case ListTypes.ScoreRanking:
                    rank = await _scoreService.SubCategoryRankingAsync(SubCategoryId, balanceType, ServiceModel);
                    break;

                case ListTypes.ScoreRankingFollowing:
                    rank = await _scoreService.FollowingRankingAsync(SubCategoryId, balanceType, ServiceModel);
                    break;
            }

            if (rank != null)
            {
                if (rank.Ranks.Count > 0)
                {
                    rank.Ranks.Items.ToList().FirstOrDefault().CornerRadius = new CornerRadius(8, 8, 0, 0);
                    rank.Ranks.Items.ToList().LastOrDefault().CornerRadius = new CornerRadius(0, 0, 8, 8);
                }
                ServiceModel = rank.Ranks;

                await base.InitializeAsync();

                if (Ranks == null || Ranks.ContestFinishDate == null)
                {
                    Ranks = new RankModel
                    {
                        ContestFinishDate = rank.ContestFinishDate
                    };

                    TimeLeftCommand.Execute(null);
                }
            }

            LoadRankEmptyMessage();

            IsBusy = false;
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private async Task ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            await PushNavigationPageAsync(nameof(ProfileView), new NavigationParameters
                {
                    {"UserName", userName }
                });

            IsBusy = false;
        }

        /// <summary>
        /// Yarışmanın biteceği zamanı verir
        /// </summary>
        private void ExecuteTimeLeftCommand()
        {
            Device.StartTimer(new TimeSpan(0, 0, 0, 1, 0), () =>
            {
                if (!IsTimerStop)
                    return IsTimerStop;

                TimeSpan diff = Ranks.ContestFinishDate - DateTime.Now;// Datetime.now telefonun tarihi yanlışsa yanlış tarih gösterir

                Ranks.TimeLeft = diff.Days +
                                        ContestParkResources.Day +
                                        diff.Hours + ContestParkResources.Hour +
                                        diff.Minutes + ContestParkResources.Minute +
                                        diff.Seconds + ContestParkResources.Seconds;

                Debug.WriteLine(Ranks.TimeLeft);
                return !(diff.Days == 0 && diff.Hours == 0 && diff.Minutes == 0 && diff.Seconds == 0);
            });
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

            RefreshCommand.Execute(null);
        }

        #endregion Methods

        #region Commands

        private ICommand _gotoProfilePageCommand;

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(async (userName) => await ExecuteGotoProfilePageCommand(userName)));

        public ICommand SegmentValueChangedCommand => new Command<int>((selectedSegmentIndex) => SegmentValueChanged(selectedSegmentIndex));
        private ICommand TimeLeftCommand => new Command(() => ExecuteTimeLeftCommand());

        #endregion Commands

        #region Navigation

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SubCategoryId")) SubCategoryId = parameters.GetValue<short>("SubCategoryId");
            if (parameters.ContainsKey("ListType")) ListType = parameters.GetValue<ListTypes>("ListType");

            base.OnNavigatedTo(parameters);
        }

        #endregion Navigation
    }
}
