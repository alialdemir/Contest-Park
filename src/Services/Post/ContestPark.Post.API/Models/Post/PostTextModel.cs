using Newtonsoft.Json;

namespace ContestPark.Post.API.Models.Post
{
    public partial class PostModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
