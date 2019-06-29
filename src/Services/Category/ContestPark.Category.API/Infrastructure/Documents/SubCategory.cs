using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Models;
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
        public string Description { get; set; }

        private long _followerCount;

        public long FollowerCount
        {
            get { return _followerCount; }
            set
            {
                if (value >= 0) _followerCount = value;
            }
        }

        public virtual ICollection<Localized> SubCategoryLocalized { get; set; } = new HashSet<Localized>();
    }
}