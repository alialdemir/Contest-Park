using ContestPark.Duel.API.Models;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ContestDate
{
    public interface IContestDateRepository
    {
        ContestDateModel ActiveContestDate();
        System.Threading.Tasks.Task<bool> AddAsync(System.DateTime startedDate, System.DateTime finishDate);
    }
}
