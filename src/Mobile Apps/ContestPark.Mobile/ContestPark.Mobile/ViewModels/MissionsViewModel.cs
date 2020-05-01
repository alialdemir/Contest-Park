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

        private string _ListViewHeader;

        public string ListViewHeader
        {
            get { return _ListViewHeader; }
            private set
            {
                _ListViewHeader = value;
                RaisePropertyChanged(() => ListViewHeader);
            }
        }

        #endregion Properties

        #region Methods

        public override Task InitializeAsync(INavigationParameters parameters = null)
        {
            MissionListCommandAsync.Execute(null);

            return base.InitializeAsync(parameters);
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
        private async Task ExecuteTakesTaskGoldCommandAsync(short missionId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            bool isSuccess = await _missionService.TakesMissionGoldAsync(missionId);
            if (isSuccess)
            {
                Items
                   .Where(p => p.MissionId == missionId)
                   .FirstOrDefault()
                   .MissionStatus = true;
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
        private void SetListViewHeader(byte completeMissionCount)
        {
            ListViewHeader = Items.Count.ToString() + "/" + completeMissionCount;
        }

        /// <summary>
        /// Görev listesini getirir
        /// </summary>
        private async Task ExecuteMissionListCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            MissionListModel missionListModel = await _missionService.MissionListAsync(ServiceModel);
            if (missionListModel.Items != null)
            {
                Items.AddRange(missionListModel.Items);

                SetListViewHeader(missionListModel.CompleteMissionCount);
            }

            ServiceModel = missionListModel;

            IsBusy = false;
        }

        #endregion Methods

        #region Command

        private ICommand MissionListCommandAsync => new CommandAsync(ExecuteMissionListCommandAsync);

        private ICommand _takesTaskGoldCommand;

        public ICommand TakesTaskGoldCommand
        {
            get { return _takesTaskGoldCommand ?? (_takesTaskGoldCommand = new CommandAsync<short>(ExecuteTakesTaskGoldCommandAsync)); }
        }

        #endregion Command
    }
}
