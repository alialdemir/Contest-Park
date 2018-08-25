using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Category.Entities.Mapper
{
    public class SubCategoryLangMapper : ClassMapper<SubCategoryLangEntity>
    {
        public SubCategoryLangMapper()
        {
            Table("SubCategoryLangs");

            AutoMap();
        }
    }
}