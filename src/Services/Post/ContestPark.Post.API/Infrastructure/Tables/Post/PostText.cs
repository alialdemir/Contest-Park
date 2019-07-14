using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Tables.Post
{
    public partial class Post
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
