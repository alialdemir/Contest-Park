using System.Threading.Tasks;

namespace ContestPark.Category.API.Infrastructure.Repositories.UserLevel
{
    public interface IUserLevelRepository
    {
        short GetUserLevel(string userId, short subCategoryId);

        Task<bool> UpdateLevel(string userId, short subCategoryId, byte exp);
    }
}
