using DapperExtensions.Mapper;

namespace ContestPark.Infrastructure.Duel.Entities.Mapper
{
    public class DuelEntityMapper : ClassMapper<DuelEntity>
    {
        public DuelEntityMapper()
        {
            Table("Duels");

            AutoMap();
        }
    }
}