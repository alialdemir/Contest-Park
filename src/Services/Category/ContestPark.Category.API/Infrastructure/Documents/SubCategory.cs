using ContestPark.Core.CosmosDb.Models;
using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class SubCategory : DocumentBase
    {
        public string PicturePath { get; set; }
        public bool Visibility { get; set; }
        public byte DisplayOrder { get; set; }
        public int Price { get; set; }
        public string DisplayPrice { get; set; }

        public virtual ICollection<SubCategoryLang> SubCategoryLangs { get; set; } = new HashSet<SubCategoryLang>();
    }
}