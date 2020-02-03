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

        private RankingModel _first;

        public RankingModel First
        {
            get { return _first; }
            set
            {
                _first = value;

                RaisePropertyChanged(() => First);
            }
        }

        private RankingModel _secound;

        public RankingModel Secound
        {
            get { return _secound; }
            set
            {
                _secound = value;

                RaisePropertyChanged(() => Secound);
            }
        }

        private RankingModel _third;

        public RankingModel Third
        {
            get { return _third; }
            set
            {
                _third = value;

                RaisePropertyChanged(() => Third);
            }
        }
    }
}
