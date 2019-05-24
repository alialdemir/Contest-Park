using ContestPark.Mobile.AppResources;
using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.ViewModels.Base;
using Newtonsoft.Json;

namespace ContestPark.Mobile.Models.Post
{
    public partial class PostModel
    {
        private int _bet;
        private string competitorProfilePicturePath = DefaultImages.DefaultProfilePicture;
        private string founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        public int Bet
        {
            get { return _bet; }
            set
            {
                _bet = value;
                RaisePropertyChanged(() => Bet);
            }
        }

        [JsonIgnore]
        public string CompetitorColor
        {
            get
            {
                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount)
                    return "#FFC200";//sarı
                else if (CompetitorTrueAnswerCount > FounderTrueAnswerCount)
                    return "#017d46";//yeşil

                return "#993232";//kırmız
            }
        }

        public string CompetitorFullName { get; set; }

        public string CompetitorProfilePicturePath
        {
            get
            {
                if (!PostType.HasFlag(PostTypes.Contest))
                    return string.Empty;

                return competitorProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) competitorProfilePicturePath = value;
            }
        }

        public byte CompetitorTrueAnswerCount { get; set; }
        public string CompetitorUserName { get; set; }

        [JsonIgnore]
        public string CompetitorWinnerOrLose
        {
            get
            {
                if (!PostType.HasFlag(PostTypes.Contest))
                    return string.Empty;

                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount)
                    return ContestParkResources.Tie;
                else if (CompetitorTrueAnswerCount > FounderTrueAnswerCount)
                    return ContestParkResources.Winning;

                return ContestParkResources.Lose;
            }
        }

        public int DuelId { get; set; }

        [JsonIgnore]
        public string FounderColor
        {
            get
            {
                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount)
                    return "#FFC200";//sarı
                else if (FounderTrueAnswerCount > CompetitorTrueAnswerCount)
                    return "#017d46";//yeşil

                return "#993232";//kırmız
            }
        }

        public string FounderFullName { get; set; }

        public string FounderProfilePicturePath
        {
            get
            {
                return founderProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) founderProfilePicturePath = value;
            }
        }

        public byte FounderTrueAnswerCount { get; set; }
        public string FounderUserName { get; set; }

        [JsonIgnore]
        public string FounderWinnerOrLose
        {
            get
            {
                if (!PostType.HasFlag(PostTypes.Contest))
                    return string.Empty;

                if (FounderTrueAnswerCount == CompetitorTrueAnswerCount)
                    return ContestParkResources.Tie;
                else if (FounderTrueAnswerCount > CompetitorTrueAnswerCount)
                    return ContestParkResources.Winning;

                return ContestParkResources.Lose;
            }
        }

        public short SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string SubCategoryPicturePath { get; set; }
    }
}