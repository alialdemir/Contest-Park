using System.Collections.Generic;

namespace ContestPark.Category.API.Model
{
    public class SearchResponseModel<T> : IndexResponseModel
    {
        public IEnumerable<T> Documents { get; set; }
    }
}