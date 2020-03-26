using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.Duel.DuelResult;
using ContestPark.Mobile.Models.Duel.InviteDuel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public interface IDuelService
    {
        Task AddOpponent(StandbyModeModel standbyModeModel);

        Task<bool> DuelEscape(int duelId);

        Task<DuelResultModel> DuelResult(int duelId);

        Task ExitStandMode(StandbyModeModel standbyModeModel);

        Task<string[]> RandomUserProfilePictures();

        Task<bool> StandbyMode(StandbyModeModel standbyModeModel);

        Task<OpponentUserModel> InviteDuel(InviteDuelModel inviteDuel);

        Task<bool> AcceptInviteDuel(AcceptInviteDuelModel acceptInvite);

        Task<bool> DuelCancel();
    }
}
