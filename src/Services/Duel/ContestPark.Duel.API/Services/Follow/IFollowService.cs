namespace ContestPark.Duel.API.Services.Follow
{
    public interface IFollowService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetFollowingUserIds(string userId, Core.Database.Models.PagingModel pagingModel);
    }
}
