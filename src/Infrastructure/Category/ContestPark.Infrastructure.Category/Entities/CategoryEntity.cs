using ContestPark.Core.Model;
using System;

namespace ContestPark.Infrastructure.Category.Entities
{
    public class CategoryEntity : EntityBase
    {
        public Int16 CategoryId { get; set; }
        public bool Visibility { get; set; }
        public byte Order { get; set; }
        public string Color { get; set; }
    }
}