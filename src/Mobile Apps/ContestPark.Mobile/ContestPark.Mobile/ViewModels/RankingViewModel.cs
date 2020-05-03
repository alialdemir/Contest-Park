using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
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

        private ListTypes _listType;

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

        private short _subCategoryId;

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            parameters.TryGetValue("SubCategoryId", out _subCategoryId);
            parameters.TryGetValue("ListType", out _listType);

            GetRankingCommand.Execute(null);

            base.Initialize(parameters);
        }

        /// <summary>
        /// Profile sayfasına git
        /// </summary>
        /// <param name="userName">Profili açılacak kullanıcının kullanıcı adı</param>
        private void ExecuteGotoProfilePageCommand(string userName)
        {
            if (IsBusy || string.IsNullOrEmpty(userName))
                return;

            IsBusy = true;

            NavigateToAsync<ProfileView>(new NavigationParameters
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
            switch (_listType)
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
                case 1: _listType = ListTypes.ScoreRankingFollowing; break;
                default: _listType = ListTypes.ScoreRanking; break;
            }

            LoadRankEmptyMessage();

            RefreshCommand.Execute(null);
        }

        /// <summary>
        /// Sıralama listesini getirir
        /// </summary>
        private async Task ExecuteGetRankingCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            BalanceTypes balanceType = BalanceTypes.Gold;

            RankModel rank = null;

            switch (_listType)
            {
                case ListTypes.ScoreRanking:
                    rank = await _scoreService.SubCategoryRankingAsync(_subCategoryId, balanceType, ServiceModel);
                    break;

                case ListTypes.ScoreRankingFollowing:
                    rank = await _scoreService.FollowingRankingAsync(_subCategoryId, balanceType, ServiceModel);
                    break;
            }

            if (rank != null)
            {
                if (Ranks == null || Ranks.ContestFinishDate == null || !Items.Any())
                {
                    Ranks = new RankModel
                    {
                        ContestFinishDate = rank.ContestFinishDate
                    };

                    int rankCount = rank.Ranks.Items.Count();
                    if (rankCount >= 1)
                    {
                        Ranks.First = rank.Ranks.Items.ToList()[0];
                    }

                    if (rankCount >= 2)
                    {
                        Ranks.Secound = rank.Ranks.Items.ToList()[1];
                    }

                    if (rankCount >= 3)
                    {
                        Ranks.Third = rank.Ranks.Items.ToList()[2];
                    }

                    if (rankCount >= 4)
                    {
                        rank.Ranks.Items = rank.Ranks.Items.Skip(3).ToList();

                        ServiceModel = rank.Ranks;
                    }

                    if (rankCount <= 4 && rankCount != 0)
                        IsShowEmptyMessage = false;

                    TimeLeftCommand.Execute(null);
                }
                else
                {
                    ServiceModel = rank.Ranks;
                }
            }

            LoadRankEmptyMessage();

            IsBusy = false;

            IsBusy = false;
        }

        #endregion Methods

        #region Commands

        private ICommand GetRankingCommand => new CommandAsync(ExecuteGetRankingCommandAsync);

        private ICommand _gotoProfilePageCommand;

        public ICommand GotoProfilePageCommand =>
            _gotoProfilePageCommand ?? (_gotoProfilePageCommand = new Command<string>(ExecuteGotoProfilePageCommand));

        public ICommand SegmentValueChangedCommand => new Command<int>(SegmentValueChanged);
        private ICommand TimeLeftCommand => new Command(ExecuteTimeLeftCommand);

        #endregion Commands
    }
}
