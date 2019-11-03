using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.Category
{
    public class CategoryInsertModel
    {
        public byte DisplayOrder { get; set; }

        public List<LocalizedModel> LocalizedModels { get; set; }
    }
}
