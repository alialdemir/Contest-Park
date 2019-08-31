using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Ranking;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Score
{
    public interface IScoreService
    {
        Task<RankModel> SubCategoryRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel);

        Task<RankModel> FollowingRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel);
    }
}
