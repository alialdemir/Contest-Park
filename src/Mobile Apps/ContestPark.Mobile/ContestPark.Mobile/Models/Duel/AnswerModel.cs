using ContestPark.Mobile.Enums;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Duel
{
    public class AnswerModel : INotifyPropertyChanged
    {
        public Languages Language { get; set; }

        public bool IsCorrect { get; set; }

        public string Answers { get; set; }

        private Color _color = Color.FromHex("#FFFFFF");

        [JsonIgnore]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}