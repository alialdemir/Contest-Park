using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Category.Entities.Mapper
{
    public class OpenSubCategoryMapper : ClassMapper<OpenSubCategoryEntity>
    {
        public OpenSubCategoryMapper()
        {
            Table("OpenSubCategories");

            AutoMap();
        }
    }
}