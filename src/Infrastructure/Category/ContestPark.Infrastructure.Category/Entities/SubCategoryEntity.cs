using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Category.Entities
{
    public partial class SubCategoryEntity : EntityBase
    {
        public Int16 SubCategoryId { get; set; }
        public Int16 CategoryId { get; set; }
        public string PictuePath { get; set; }
        public bool Visibility { get; set; }
        public byte Order { get; set; }
        public int Price { get; set; }
        public string DisplayPrice { get; set; }
    }
}