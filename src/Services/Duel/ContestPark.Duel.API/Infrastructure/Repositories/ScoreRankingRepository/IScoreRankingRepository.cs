using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository
{
    public interface IScoreRankingRepository
    {
        ServiceModel<RankModel> GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, short contestDateId, PagingModel pagingModel);
    }
}
