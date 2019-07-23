using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public interface IDuelRepository
    {
        Task<int?> Insert(Tables.Duel duel);
    }
}
