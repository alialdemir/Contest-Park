using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Category.Entities.Mapper
{
    public class CategoryLangMapper : ClassMapper<CategoryLangEntity>
    {
        public CategoryLangMapper()
        {
            Table("CategoryLangs");

            AutoMap();
        }
    }
}