using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Core.Database.Models
{
    public class ServiceModel<T> : PagingModel
    {
        [JsonProperty(Order = 1)]
        public bool HasNextPage { get; set; }

        [JsonProperty(Order = 2)]
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
