using ContestPark.Core.CosmosDb.Models;
using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class SubCategory : DocumentBase
    {
        public string PictuePath { get; set; }
        public bool Visibility { get; set; }
        public byte Order { get; set; }
        public int Price { get; set; }
        public string DisplayPrice { get; set; }

        public virtual ICollection<SubCategoryLang> SubCategoryLangs { get; set; } = new HashSet<SubCategoryLang>();
    }
}