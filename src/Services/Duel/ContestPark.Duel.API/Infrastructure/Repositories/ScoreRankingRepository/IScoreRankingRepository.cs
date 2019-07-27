using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository
{
    public interface IScoreRankingRepository
    {
        ServiceModel<RankModel> GetFollowingRanking(short subCategoryId, BalanceTypes balanceType, short contestDateId, System.Collections.Generic.IEnumerable<string> followingUsers, PagingModel pagingModel);

        ServiceModel<RankModel> GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, short contestDateId, PagingModel pagingModel);

        Task<bool> UpdateScoreRank(string userId, short subCategoryId, BalanceTypes balanceType, byte score);
    }
}
