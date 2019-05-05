using ContestPark.Mobile.Enums;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Duel
{
    public class AnswerModel : BaseModel
    {
        private Color _color = Color.FromHex("#FFFFFF");
        public string Answers { get; set; }

        [JsonIgnore]
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;

                RaisePropertyChanged(() => Color);
            }
        }

        public bool IsCorrect { get; set; }
        public Languages Language { get; set; }
    }
}