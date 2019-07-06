namespace ContestPark.Core.Services.Identity
{
    public interface IIdentityService
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Models.UserModel>> GetUserInfosAsync(System.Collections.Generic.IEnumerable<string> userIds);
    }
}
