using Newtonsoft.Json;
using System;

namespace ContestPark.Mobile.Models.Ranking
{
    public class TimeLeftModel : BaseModel
    {
        private string _months;

        private string _timeLeft;

        public DateTime FinsihDate { get; set; }

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