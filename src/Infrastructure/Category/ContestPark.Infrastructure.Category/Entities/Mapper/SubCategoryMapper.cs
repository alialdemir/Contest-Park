using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Category.Entities.Mapper
{
    public class SubCategoryMapper : ClassMapper<SubCategoryEntity>
    {
        public SubCategoryMapper()
        {
            Table("SubCategories");

            AutoMap();
        }
    }
}