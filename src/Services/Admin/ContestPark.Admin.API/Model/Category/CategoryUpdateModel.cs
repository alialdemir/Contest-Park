using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.Category
{
    public class CategoryUpdateModel
    {
        public short CategoryId { get; set; }

        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }

        public List<LocalizedModel> LocalizedModels { get; set; }
    }
}
