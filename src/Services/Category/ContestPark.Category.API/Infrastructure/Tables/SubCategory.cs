using ContestPark.Core.Database.Models;
using Dapper;

namespace ContestPark.Category.API.Infrastructure.Tables
{
    [Table("SubCategories")]
    public class SubCategory : EntityBase
    {
        [Key]
        public short SubCategoryId { get; set; }

        private long _followerCount;

        public byte DisplayOrder { get; set; }

        public string DisplayPrice { get; set; }

        public long FollowerCount
        {
            get { return _followerCount; }
            set
            {
                if (value >= 0) _followerCount = value;
            }
        }

        public string PicturePath { get; set; }

        public decimal Price { get; set; }

        public bool Visibility { get; set; }
    }
}
