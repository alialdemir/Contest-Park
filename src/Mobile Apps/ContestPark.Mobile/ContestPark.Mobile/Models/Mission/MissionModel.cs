namespace ContestPark.Mobile.Models.Mission
{
    public class MissionModel : BaseModel
    {
        private bool missionStatus;
        public int Gold { get; set; }
        public bool IsCompleteMission { get; set; }
        public string MissionDescription { get; set; }
        public short MissionId { get; set; }
        public string MissionName { get; set; }
        public string MissionPicturePath { get; set; }

        public bool MissionStatus
        {
            get { return missionStatus; }
            set
            {
                missionStatus = value;

                RaisePropertyChanged(() => MissionStatus);
            }
        }
    }
}