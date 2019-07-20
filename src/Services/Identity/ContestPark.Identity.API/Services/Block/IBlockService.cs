using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services.Block
{
    public interface IBlockService
    {
        Task<bool> BlockedStatusAsync(string userId1, string userId2);
    }
}
