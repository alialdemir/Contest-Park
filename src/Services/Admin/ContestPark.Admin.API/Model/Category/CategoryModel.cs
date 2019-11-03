using System;

namespace ContestPark.Admin.API.Model.Category
{
    public class CategoryModel
    {
        public short CategoryId { get; set; }

        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }

        public string CategoryName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
