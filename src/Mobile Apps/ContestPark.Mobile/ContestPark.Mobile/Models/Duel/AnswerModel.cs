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

        public Languages Language { get; set; }
        private bool _isLeftArrowVisible;

        public bool IsLeftArrowVisible
        {
            get { return _isLeftArrowVisible; }
            set
            {
                _isLeftArrowVisible = value;
                RaisePropertyChanged(() => IsLeftArrowVisible);
            }
        }

        private bool _isRightArrowVisible;

        public bool IsRightArrowVisible
        {
            get { return _isRightArrowVisible; }
            set
            {
                _isRightArrowVisible = value;
                RaisePropertyChanged(() => IsRightArrowVisible);
            }
        }
    }
}
