using ContestPark.EventBus.Events;
using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.IntegrationEvents.Events
{
    public class NewPostAddedIntegrationEvent : IntegrationEvent
    {
        #region Constructor

        public NewPostAddedIntegrationEvent(PostTypes postType,
                                          string ownerUserId,
                                          decimal? bet,
                                          string competitorUserId,
                                          byte? competitorTrueAnswerCount,
                                          string duelId,
                                          string founderUserId,
                                          byte? founderTrueAnswerCount,
                                          string subCategoryId,
                                          string picturePath,
                                          string description,
                                          PostImageTypes? postImageType)
        {
            PostType = postType;
            OwnerUserId = ownerUserId;
            Bet = bet;
            CompetitorUserId = competitorUserId;
            CompetitorTrueAnswerCount = competitorTrueAnswerCount;
            DuelId = duelId;
            FounderUserId = founderUserId;
            FounderTrueAnswerCount = founderTrueAnswerCount;
            SubCategoryId = subCategoryId;
            PicturePath = picturePath;
            Description = description;
            PostImageType = postImageType;
        }

        #endregion Constructor

        #region Post

        public PostTypes PostType { get; set; }

        public string OwnerUserId { get; set; }

        #endregion Post

        #region Post contest

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Bet { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitorUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? CompetitorTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DuelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FounderUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? FounderTrueAnswerCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryId { get; set; }

        #endregion Post contest

        #region Post image

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PostImageTypes? PostImageType { get; set; }

        #endregion Post image

        #region Post text

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        #endregion Post text
    }
}