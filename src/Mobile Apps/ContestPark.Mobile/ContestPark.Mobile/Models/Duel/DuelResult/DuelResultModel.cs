using ContestPark.Mobile.AppResources;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ContestPark.Mobile.Models.Duel.DuelResult
{
    public class DuelResultModel
    {
        private string _winnerOrLoseText = "Sen kazandın!";
        private string _winnerOrLoseTextColor = "Green";

        public byte FinishBonus { get; set; }

        [JsonIgnore]
        public string FounderColor
        {
            get
            {
                if (FounderScore > OpponentScore)
                {
                    return GetHexString("Green");
                }
                else if (FounderScore < OpponentScore)
                {
                    return GetHexString("Red");
                }
                else
                {
                    return GetHexString("Primary");
                }
            }
        }

        public string FounderFullName { get; set; }
        public short FounderLevel { get; set; }
        public string FounderProfilePicturePath { get; set; }
        public byte FounderScore { get; set; }
        public string FounderUserId { get; set; }
        public string FounderUserName { get; set; }
        public int Gold { get; set; }
        public bool IsFounder { get; set; }
        public byte MatchScore { get; set; }

        [JsonIgnore]
        public string OpponentColor
        {
            get
            {
                if (OpponentScore > FounderScore)
                {
                    return GetHexString("Green");
                }
                else if (OpponentScore < FounderScore)
                {
                    return GetHexString("Red");
                }
                else
                {
                    return GetHexString("Primary");
                }
            }
        }

        public string OpponentFullName { get; set; }
        public short OpponentLevel { get; set; }
        public string OpponentProfilePicturePath { get; set; }
        public byte OpponentScore { get; set; }
        public string OpponentUserId { get; set; }
        public string OpponentUserName { get; set; }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        public string SubCategoryPicturePath { get; set; }

        [JsonIgnore]
        public int TotalXp
        {
            get { return FinishBonus + VictoryBonus + MatchScore; }
        }

        public byte VictoryBonus { get; set; }

        [JsonIgnore]
        public string WinnerOrLoseText { get; set; }

        [JsonIgnore]
        public string WinnerOrLoseTextColor
        {
            get { return _winnerOrLoseTextColor; }
        }

        public string GetHexString(string colorName)
        {
            Color color = (Color)ContestParkApp.Current.Resources[colorName];

            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
    }
}