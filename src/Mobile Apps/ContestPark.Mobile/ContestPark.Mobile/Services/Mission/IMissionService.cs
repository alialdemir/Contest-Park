using ContestPark.Mobile.Models.Mission;
using ContestPark.Mobile.Models.PagingModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Mission
{
    public interface IMissionService
    {
        Task<MissionListModel> MissionListAsync(PagingModel pagingModel);

        Task<bool> TakesMissionGoldAsync(short missionId);
    }
}