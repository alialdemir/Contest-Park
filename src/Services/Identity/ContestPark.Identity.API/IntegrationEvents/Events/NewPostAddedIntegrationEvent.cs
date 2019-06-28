using ContestPark.EventBus.Events;
using Newtonsoft.Json;

namespace ContestPark.Identity.API.IntegrationEvents.Events
{
    public class NewPostAddedIntegrationEvent : IntegrationEvent
    {
        #region Enum

        public enum PostTypes
        {
            Image = 1,
            Contest = 2,
            Text = 3,
        }

        public enum PostImageTypes
        {
            ProfileImage = 1,
            CoverImage = 2,
            UploadedImage = 3
        }

        #endregion Enum

        #region Constructor

        public NewPostAddedIntegrationEvent(PostTypes postType,
            PostImageTypes postImageType,
                                          string ownerUserId,
                                          string picturePath)
        {
            PostType = postType;
            PostImageType = postImageType;
            OwnerUserId = ownerUserId;
            PicturePath = picturePath;
        }

        #endregion Constructor

        #region Post

        public PostTypes PostType { get; set; }

        public string OwnerUserId { get; set; }

        #endregion Post

        #region Post image

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PostImageTypes PostImageType { get; set; }

        #endregion Post image
    }
}