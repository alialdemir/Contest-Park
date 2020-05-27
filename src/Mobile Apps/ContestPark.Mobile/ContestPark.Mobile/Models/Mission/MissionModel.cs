using ContestPark.Mobile.Enums;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Mission
{
    public class MissionModel : BaseModel
    {
        private bool _isCompleteMission;
        //  private bool _isMissionCompleted;

        public byte MissionId { get; set; }

        public string Description { get; set; }

        public decimal Reward { get; set; }

        public BalanceTypes RewardBalanceType { get; set; }

        public string PicturePath { get; set; }

        public MissionTime MissionTime { get; set; }

        public bool IsCompleteMission
        {
            get { return _isCompleteMission; }
            set
            {
                _isCompleteMission = value;
                RaisePropertyChanged(() => IsCompleteMission);
                //RaisePropertyChanged(() => MissionBorderColor);
            }
        }

        //public bool IsMissionCompleted
        //{
        //    get { return _isMissionCompleted; }
        //    set
        //    {
        //        _isMissionCompleted = value;

        //        RaisePropertyChanged(() => IsMissionCompleted);
        //    }
        //}

        public Color MissionBorderColor
        {
            get { return IsCompleteMission ? Color.FromHex("#7CF5BA") : Color.FromHex("#ffffff"); }
        }
    }
}
