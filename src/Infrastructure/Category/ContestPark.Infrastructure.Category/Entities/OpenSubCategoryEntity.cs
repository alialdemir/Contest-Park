using System;
using ContestPark.Core.Model;

namespace ContestPark.Infrastructure.Category.Entities
{
    public class OpenSubCategoryEntity : EntityBase
    {
        public int OpenSubCategoryId { get; set; }
        public string UserId { get; set; }
        public Int16 SubCategoryId { get; set; }
    }
}