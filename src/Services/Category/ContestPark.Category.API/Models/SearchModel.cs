using Newtonsoft.Json;

namespace ContestPark.Category.API.Models
{
    public class SearchModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayPrice { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFollowing { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Price { get; set; }

        public SearchTypes SearchType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short SubCategoryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSubCategoryOpen { get; set; }
    }
}
