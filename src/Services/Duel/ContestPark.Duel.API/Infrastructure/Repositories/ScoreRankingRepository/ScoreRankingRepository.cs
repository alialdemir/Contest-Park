using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository
{
    public class ScoreRankingRepository : IScoreRankingRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.ScoreRanking> _scoreRankingRepository;

        #endregion Private Variables

        #region Constructor

        public ScoreRankingRepository(IRepository<Tables.ScoreRanking> scoreRankingRepository)
        {
            _scoreRankingRepository = scoreRankingRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Alt kategori id ve bakiye tipine göre skor sıralama listesi verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tip</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori sıralaması</returns>
        public ServiceModel<RankModel> GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, short contestDateId, PagingModel pagingModel)
        {
            string sql = @"SELECT CASE
                                  WHEN @balanceType=1 THEN sr.TotalGoldScore
                                  WHEN @balanceType=2 THEN sr.TotalMoneyScore
                                  END AS TotalScore,
                                  sr.UserId
                        FROM ScoreRankings sr
                        WHERE sr.ContestDateId = @contestDateId
                           AND sr.SubCategoryId = @subCategoryId
                        ORDER BY
                        CASE
                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                        END DESC";

            return _scoreRankingRepository.ToServiceModel<RankModel>(sql, new
            {
                subCategoryId,
                balanceType,
                contestDateId
            }, pagingModel: pagingModel);
        }

        #endregion Methods
    }
}
