using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Category.Entities.Mapper
{
    public class CategoryMapper : ClassMapper<CategoryEntity>
    {
        public CategoryMapper()
        {
            Table("Categories");

            AutoMap();
        }
    }
}