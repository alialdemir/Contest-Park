using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Models.ServiceModel;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Score
{
    public interface IScoreService
    {
        Task<ServiceModel<RankingModel>> SubCategoryRankingAsync(short subCategoryId, PagingModel pagingModel);

        Task<ServiceModel<RankingModel>> FollowingRankingAsync(short subCategoryId, PagingModel pagingModel);

        Task<TimeLeftModel> GetTimeLeft(short subCategoryId);
    }
}