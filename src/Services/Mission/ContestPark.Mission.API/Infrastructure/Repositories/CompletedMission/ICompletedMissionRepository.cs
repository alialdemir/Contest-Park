using System.Threading.Tasks;

namespace ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission
{
    public interface ICompletedMissionRepository
    {
        Task<bool> Add(string userId, byte missionId);

        byte CompletedMissionCount(string userId);

        bool IsMissionCompleted(string userId, byte missionId);

        bool TakesMissionReward(string userId, byte missionId);
    }
}
