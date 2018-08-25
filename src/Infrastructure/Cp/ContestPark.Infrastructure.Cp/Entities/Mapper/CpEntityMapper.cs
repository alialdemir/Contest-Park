using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Cp.Entities.Mapper
{
    public class CpEntityMapper : ClassMapper<CpEntity>
    {
        public CpEntityMapper()
        {
            Table("Cps");

            AutoMap();
        }
    }
}