using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.Mission;
using ContestPark.Mobile.Services.Mission;
using ContestPark.Mobile.ViewModels.Base;
using Prism.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ContestPark.Mobile.ViewModels
{
    public class MissionsViewModel : ViewModelBase<MissionModel>
    {
        #region Private varaibles

        private readonly IMissionService _missionService;

        #endregion Private varaibles

        #region Constructor

        public MissionsViewModel(IMissionService missionService)
        {
            Title = ContestParkResources.Missions;

            _missionService = missionService;
        }

        #endregion Constructor

        #region Properties

        private byte _completedMissionCount;

        public byte CompletedMissionCount
        {
            get { return _completedMissionCount; }
            set
            {
                _completedMissionCount = value;
                RaisePropertyChanged(() => CompletedMissionCount);
            }
        }

        private string _listViewHeader;

        public string ListViewHeader
        {
            get { return _listViewHeader; }
            private set
            {
                _listViewHeader = value;
                RaisePropertyChanged(() => ListViewHeader);
            }
        }

        #endregion Properties

        #region Methods

        public override void Initialize(INavigationParameters parameters = null)
        {
            MissionListCommandAsync.Execute(null);

            base.Initialize(parameters);
        }

        protected override void Reflesh()
        {
            ListViewHeader = string.Empty;

            base.Reflesh();
        }

        /// <summary>
        /// Yapılmış olan görevin altınını alma
        /// </summary>
        /// <param name="missionId">Görevin Id'si</param>
        /// <returns></returns>
        private async Task ExecuteTakesTaskGoldCommandAsync(byte missionId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            bool isSuccess = await _missionService.TakesMissionRewardAsync(missionId);
            if (isSuccess)
            {
                //Items
                //   .Where(p => p.MissionId == missionId)
                //   .FirstOrDefault()
                //   .IsMissionCompleted = true;
                Items
                   .Where(p => p.MissionId == missionId)
                   .FirstOrDefault()
                   .IsCompleteMission = true;

                // Header da bulunan tamamlanan görev sayısıı güncelledik
                string[] headerSplit = ListViewHeader.Split('/');
                if (headerSplit.Length == 2)
                {
                    byte completeMissionCount = Convert.ToByte(headerSplit[1]);

                    SetListViewHeader(completeMissionCount++);
                }
            }

            IsBusy = false;
        }

        /// <summary>
        /// Tamamlanan görev sayısını header da günceller
        /// </summary>
        private void SetListViewHeader(byte completedMissionCount)
        {
            ListViewHeader = completedMissionCount + "/" + Items.Count.ToString();
            CompletedMissionCount = completedMissionCount;
        }

        /// <summary>
        /// Görev listesini getirir
        /// </summary>
        private async Task ExecuteMissionListCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            MissionServiceModel missions = await _missionService.MissionListAsync(ServiceModel);
            if (missions != null && missions.Items != null)
            {
                Items.AddRange(missions.Items);

                SetListViewHeader(missions.CompletedMissionCount);
            }

            ServiceModel = missions;

            IsBusy = false;
        }

        #endregion Methods

        #region Command

        private ICommand MissionListCommandAsync => new CommandAsync(ExecuteMissionListCommandAsync);

        private ICommand _takesTaskGoldCommand;

        public ICommand TakesTaskGoldCommand
        {
            get { return _takesTaskGoldCommand ?? (_takesTaskGoldCommand = new CommandAsync<byte>(ExecuteTakesTaskGoldCommandAsync)); }
        }

        #endregion Command
    }
}
