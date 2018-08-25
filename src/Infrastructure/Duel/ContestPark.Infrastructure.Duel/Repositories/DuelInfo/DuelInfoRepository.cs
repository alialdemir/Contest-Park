using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Infrastructure.Duel.Entities;
using Dapper;
using System.Linq;

namespace ContestPark.Infrastructure.Duel.Repositories.DuelInfo
{
    public class DuelInfoRepository : DapperRepositoryBase<DuelInfoEntity>, IDuelInfoRepository
    {
        #region Constructor

        public DuelInfoRepository(ISettingsBase settings) : base(settings)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Gelen düello id ait kayıt var mı kontrol eder
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello  bilgisi entity</returns>
        public DuelInfoEntity GetDuelInfoByDuelId(int duelId, int questionInfoId)
        {
            string sql = "select * from [DuelInfos] as [di] where [di].[DuelId]=@duelId and [di].[QuestionInfoId]=@questionInfoId";

            return Connection.Query<DuelInfoEntity>(sql, new { duelId, questionInfoId }).FirstOrDefault();
        }

        /// <summary>
        /// Düelloda 7 sorudan fazla soru sorulduysa false sorulmadıysa true döner
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <param name="gameCount">Kaç soru soralabileceği</param>
        /// <returns>Oyun bitti ise true bitmedi ise false</returns>
        public bool IsGameEnd(int duelId, byte gameCount = 7)
        {
            string sql = @"SELECT (CASE WHEN EXISTS(
                           SELECT NULL AS [EMPTY] FROM [DuelInfos] as [di]
                           where [di].[DuelId]=@duelId
                           having count([di].[DuelId]) > @gameCount
                           ) THEN 1 ELSE 0 END)";

            return Connection.Query<bool>(sql, new { duelId, gameCount }).FirstOrDefault();

            #endregion Methods
        }
    }
}