namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public interface IDuelRepository
    {
        System.Threading.Tasks.Task<bool> Insert(Tables.Duel duel);
    }
}