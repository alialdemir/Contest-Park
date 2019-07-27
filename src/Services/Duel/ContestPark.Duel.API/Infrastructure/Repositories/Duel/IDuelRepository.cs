using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public interface IDuelRepository
    {
        Tables.Duel GetDuelByDuelId(int duelId);
        Task<int?> Insert(Tables.Duel duel);
        Task<bool> UpdateAsync(Tables.Duel duel);
    }
}
