using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Cp.Entities.Mapper
{
    public class CpInfoEntityMapper : ClassMapper<CpInfoEntity>
    {
        public CpInfoEntityMapper()
        {
            Table("CpInfos");

            AutoMap();
        }
    }
}