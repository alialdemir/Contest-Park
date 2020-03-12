using ContestPark.Core.Models;
using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Models.Post
{
    public partial class PostModel
    {
        private string competitorProfilePicturePath = DefaultImages.DefaultProfilePicture;
        private string founderProfilePicturePath = DefaultImages.DefaultProfilePicture;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Bet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BalanceTypes BalanceType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DuelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? SubCategoryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? CompetitorTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? FounderTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryPicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorFullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorProfilePicturePath
        {
            get
            {
                if (PostType != PostTypes.Contest)
                    return null;

                return competitorProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) competitorProfilePicturePath = value;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderFullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderProfilePicturePath
        {
            get
            {
                if (PostType != PostTypes.Contest)
                    return null;

                return founderProfilePicturePath;
            }
            set
            {
                if (!string.IsNullOrEmpty(value)) founderProfilePicturePath = value;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserName { get; set; }
    }
}
