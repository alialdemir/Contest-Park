using System.ComponentModel;

namespace ContestPark.Mobile.Models.Mission
{
    public class MissionModel : BaseModel, INotifyPropertyChanged
    {
        public string MissionName { get; set; }

        public int Gold { get; set; }

        public string MissionPicturePath { get; set; }

        private bool missionStatus;

        public bool MissionStatus
        {
            get { return missionStatus; }
            set
            {
                missionStatus = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MissionStatus)));
            }
        }

        public string MissionDescription { get; set; }

        public short MissionId { get; set; }

        public bool IsCompleteMission { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}