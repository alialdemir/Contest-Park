using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Models;
using System.Collections.Generic;

namespace ContestPark.Category.API.Infrastructure.Documents
{
    public class Category : DocumentBase
    {
        public bool Visibility { get; set; }

        public byte DisplayOrder { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; } = new HashSet<SubCategory>();
        public virtual ICollection<Localized> CategoryLocalized { get; set; } = new HashSet<Localized>();
    }
}