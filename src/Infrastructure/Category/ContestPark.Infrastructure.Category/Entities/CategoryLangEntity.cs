using ContestPark.Core.Enums;
using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Category.Entities
{
    public class CategoryLangEntity : EntityBase
    {
        public Int16 CategoryLangId { get; set; }
        public string CategoryName { get; set; }
        public Int16 CategoryId { get; set; }
        public Languages LanguageId { get; set; }
    }
}