using ContestPark.Core.CosmosDb.Models;
using Newtonsoft.Json;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    public partial class Post : DocumentBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}