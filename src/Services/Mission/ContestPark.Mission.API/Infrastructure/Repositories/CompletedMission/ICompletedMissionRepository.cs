namespace ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission
{
    public interface ICompletedMissionRepository
    {
        byte CompletedMissionCount(string userId);
    }
}
