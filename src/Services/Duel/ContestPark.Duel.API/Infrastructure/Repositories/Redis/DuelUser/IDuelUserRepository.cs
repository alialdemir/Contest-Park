using ContestPark.Duel.API.Models;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.DuelUser
{
    public interface IDuelUserRepository
    {
        bool Delete(DuelUserModel duelUser);

        DuelUserModel GetDuelUser(DuelUserModel duelUser);

        bool Insert(DuelUserModel duelUser);
    }
}
