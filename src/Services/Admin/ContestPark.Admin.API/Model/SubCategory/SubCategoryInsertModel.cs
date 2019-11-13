using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ContestPark.Admin.API.Model.SubCategory
{
    public class SubCategoryInsertModel
    {
        public byte DisplayOrder { get; set; }

        public decimal Price { get; set; }

        public string PicturePath { get; set; }

        public List<LocalizedModel> LocalizedModels { get; set; }

        public List<short> CategoryIds { get; set; }

        public IList<IFormFile> Files { get; set; }
    }
}
