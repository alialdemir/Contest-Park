using ContestPark.Mobile.Models.ServiceModel;
using Newtonsoft.Json;
using System;

namespace ContestPark.Mobile.Models.Ranking
{
    public class RankModel : BaseModel
    {
        public ServiceModel<RankingModel> Ranks { get; set; }

        private string _months;

        private string _timeLeft;

        public DateTime ContestFinishDate { get; set; }

        [JsonIgnore]
        public string Months
        {
            get { return _months; }
            set
            {
                _months = value;

                RaisePropertyChanged(() => Months);
            }
        }

        [JsonIgnore]
        public string TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value;

                RaisePropertyChanged(() => TimeLeft);
            }
        }
    }
}
