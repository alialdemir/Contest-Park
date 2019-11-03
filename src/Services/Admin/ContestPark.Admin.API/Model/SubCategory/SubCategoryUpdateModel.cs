using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.SubCategory
{
    public class SubCategoryUpdateModel
    {
        public short SubCategoryId { get; set; }

        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }

        public decimal Price { get; set; }

        public string PicturePath { get; set; }

        public List<LocalizedModel> LocalizedModels { get; set; }

        public List<short> CategoryIds { get; set; }
    }
}
