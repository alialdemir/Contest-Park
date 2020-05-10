using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Mission.API.Models;

namespace ContestPark.Mission.API.Infrastructure.Repositories.Mission
{
    public interface IMissionRepository
    {
        ServiceModel<MissionModel> GetMissions(string userId, Languages language, PagingModel pagingModel);
    }
}
