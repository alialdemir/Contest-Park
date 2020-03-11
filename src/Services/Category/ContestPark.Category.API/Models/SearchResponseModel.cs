using System.Collections.Generic;

namespace ContestPark.Category.API.Models
{
    public class SearchResponseModel<T> : IndexResponseModel
    {
        public IEnumerable<T> Documents { get; set; }
    }
}
