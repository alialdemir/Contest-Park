using ContestPark.Core.CosmosDb.Models;
using ContestPark.Post.API.Enums;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PostImageTypes? PostImageType { get; set; }
    }
}