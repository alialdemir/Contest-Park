using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ContestPark.Mobile.Models.Ranking
{
    public class TimeLeftModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _months;

        public string Months
        {
            get { return _months; }
            set
            {
                _months = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Months)));
            }
        }

        private string _timeLeft;

        [JsonIgnore]
        public string TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                _timeLeft = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeLeft)));
            }
        }

        public DateTime FinsihDate { get; set; }
    }
}