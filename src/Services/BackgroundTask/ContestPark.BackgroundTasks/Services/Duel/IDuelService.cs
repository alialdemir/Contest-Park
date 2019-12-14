using ContestPark.BackgroundTasks.Models;
using System.Threading.Tasks;

namespace ContestPark.BackgroundTasks.Services.Duel
{
    public interface IDuelService
    {
        Task<ContestDateModel> ActiveContestDate();
    }
}
