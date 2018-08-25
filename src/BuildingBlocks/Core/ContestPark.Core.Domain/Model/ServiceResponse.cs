using Newtonsoft.Json;
using System.Collections.Generic;

namespace ContestPark.Core.Domain.Model
{
    public class ServiceResponse<T> : Paging
    {
        [JsonProperty(Order = 1)]
        public int Count { get; set; }

        [JsonProperty(Order = 2)]
        public IEnumerable<T> Items { get; set; }

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