using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Mission
{
    public class MissionModel : BaseModel
    {
        private bool _missionStatus;
        public int Gold { get; set; }

        private bool _isCompleteMission;

        public bool IsCompleteMission
        {
            get { return _isCompleteMission; }
            set
            {
                _isCompleteMission = value;
                RaisePropertyChanged(() => IsCompleteMission);
                RaisePropertyChanged(() => MissionBorderColor);
            }
        }

        public string MissionDescription { get; set; }
        public short MissionId { get; set; }
        public string MissionName { get; set; }
        public string MissionPicturePath { get; set; }

        public bool MissionStatus
        {
            get { return _missionStatus; }
            set
            {
                _missionStatus = value;

                RaisePropertyChanged(() => MissionStatus);
            }
        }

        public Color MissionBorderColor
        {
            get { return IsCompleteMission ? Color.FromHex("#7CF5BA") : Color.FromHex("#ffffff"); }
        }
    }
}
