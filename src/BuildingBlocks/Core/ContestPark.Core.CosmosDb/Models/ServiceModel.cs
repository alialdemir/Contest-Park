using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Core.CosmosDb.Models
{
    public class ServiceModel<T> : PagingModel
    {
        [JsonProperty(Order = 1)]
        public int Count { get; set; }

        [JsonProperty(Order = 2)]
        public IEnumerable<T> Items { get; set; } = new List<T>();

        [JsonIgnore]
        public bool IsLastPage
        {
            get
            {
                if (PageNumber <= 0) return true;
                return PageNumber > (Count / PageSize);
            }
        }
    }
}