using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Mobile.Models.ServiceModel
{
    public class ServiceModel<T> : PagingModel.PagingModel
    {
        [JsonProperty(Order = 1)]
        public int Count { get; set; }

        [JsonProperty(Order = 2)]
        public IEnumerable<T> Items { get; set; }

        public bool HasNextPage { get; set; }
    }
}
