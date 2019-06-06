using Newtonsoft.Json;

namespace ContestPark.Category.API.Model
{
    public class SearchModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("categoryname", NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryName { get; set; }

        [JsonProperty("displayprice", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayPrice { get; set; }

        [JsonProperty("fullname", NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        [JsonProperty("isfollow", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFollow { get; set; }

        [JsonProperty("picturepath", NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
        public int? Price { get; set; }

        [JsonProperty("searchtype")]
        public SearchTypes SearchType { get; set; }

        [JsonProperty("subcategoryid", NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryId { get; set; }

        [JsonProperty("subcategoryname", NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryName { get; set; }

        [JsonProperty("userid", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }
    }

    public enum SearchTypes : byte
    {
        Player = 1,
        Category = 2
    }
}