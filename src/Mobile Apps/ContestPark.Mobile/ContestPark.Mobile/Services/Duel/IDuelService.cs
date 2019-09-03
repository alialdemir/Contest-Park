using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public interface IDuelService
    {
        Task AddOpponent(StandbyModeModel standbyModeModel);
        Task<bool> DuelEscape(int duelId);
        Task<DuelResultModel> DuelResult(int duelId);

        //   Task<bool> DuelStartWithDuelId(string duelId);

        ///   Task<bool> DuelStartWithUserId(string userId);

        Task ExitStandMode(StandbyModeModel standbyModeModel);

        Task<string[]> RandomUserProfilePictures();

        Task<bool> StandbyMode(StandbyModeModel standbyModeModel);
    }
}
