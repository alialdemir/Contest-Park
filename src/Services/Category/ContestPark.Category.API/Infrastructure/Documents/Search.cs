using ContestPark.Category.API.Model;
using ContestPark.Core.Enums;
using Nest;
using Newtonsoft.Json;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class Search : ISearchBase
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayPrice { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFollow { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PicturePath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Price { get; set; }

        public SearchTypes SearchType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Languages? Language { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SubCategoryName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryName { get; set; }

        public CompletionField Suggest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryId { get; set; }
    }

    public interface ISearchBase
    {
        CompletionField Suggest { get; set; }
    }
}