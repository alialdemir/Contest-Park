using ContestPark.Mobile.Models.Duel;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Duel
{
    public interface IDuelService
    {
        Task<ServiceModel<string>> RandomUserProfilePictures(PagingModel pagingModel);

        Task StandbyMode(StandbyModeModel standbyModeModel);

        Task ExitStandMode(StandbyModeModel standbyModeModel);

        Task BotStandMode(BotStandbyMode botStandbyMode);
    }
}