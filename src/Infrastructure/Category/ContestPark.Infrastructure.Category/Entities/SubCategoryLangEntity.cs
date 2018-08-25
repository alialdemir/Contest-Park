using ContestPark.Core.Enums;
using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Category.Entities
{
    public class SubCategoryLangEntity : EntityBase
    {
        public Int16 SubCategoryLangId { get; set; }
        public string SubCategoryName { get; set; }
        public Languages LanguageId { get; set; }
        public Int16 SubCategoryId { get; set; }
    }
}