using ContestPark.Mobile.Models.Mission;
using ContestPark.Mobile.Models.PagingModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Mission
{
    public interface IMissionService
    {
        Task<MissionServiceModel> MissionListAsync(PagingModel pagingModel);

        Task<bool> TakesMissionRewardAsync(byte missionId);
    }
}
