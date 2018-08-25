using ContestPark.Core.Dapper;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Infrastructure.Duel.Entities;

namespace ContestPark.Infrastructure.Duel.Repositories.Duel
{
    public interface IDuelRepository : IRepository<DuelEntity>
    {
        DuelStarting GetDuelStarting(int duelId);
    }
}