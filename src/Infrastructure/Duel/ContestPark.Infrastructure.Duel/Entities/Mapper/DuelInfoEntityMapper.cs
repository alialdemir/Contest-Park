using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Duel.Entities.Mapper
{
    public class DuelInfoEntityMapper : ClassMapper<DuelInfoEntity>
    {
        public DuelInfoEntityMapper()
        {
            Table("DuelInfos");

            AutoMap();
        }
    }
}