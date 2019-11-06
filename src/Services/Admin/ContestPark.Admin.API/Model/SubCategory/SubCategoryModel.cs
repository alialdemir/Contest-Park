using System;

namespace ContestPark.Admin.API.Model.SubCategory
{
    public class SubCategoryModel
    {
        public short SubCategoryId { get; set; }

        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }

        public string SubCategoryName { get; set; }

        public byte LinkedCategories { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
