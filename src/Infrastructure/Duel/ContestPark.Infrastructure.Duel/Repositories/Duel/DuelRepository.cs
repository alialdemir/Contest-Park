using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Infrastructure.Duel.Entities;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ContestPark.Infrastructure.Duel.Repositories.Duel
{
    internal class DuelRepository : DapperRepositoryBase<DuelEntity>, IDuelRepository
    {
        #region MyRegion

        private readonly ILogger<DuelRepository> _logger;

        #endregion MyRegion

        #region Constructor

        public DuelRepository(ISettingsBase settingsBase,
                              ILogger<DuelRepository> logger) : base(settingsBase)
        {
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        public DuelStarting GetDuelStarting(int duelId)
        {
            if (duelId <= 0)
                return null;

            string sql = @"SELECT TOP 1
                       [d].[DuelId],
                       [founder].[Id] as [FounderUserId],
                       [founder].[FullName] as [FounderFullName],
                       [founder].[ProfilePicturePath] as [FounderProfilePicturePath],
                       [founder].[CoverPicturePath] as [FounderCoverPicturePath],
                       [opponent].[Id] as [OpponentUserId],
                       [opponent].[FullName] as [OpponentFullName],
                       [opponent].[ProfilePicturePath] as [OpponentProfilePicturePath],
                       [opponent].[CoverPicturePath] as [OpponentCoverPicturePath]
                       FROM [Duels] AS [d]
                       INNER JOIN [AspNetUsers] as [founder] ON [d].[FounderUserId]=[founder].[Id]
                       INNER JOIN [AspNetUsers] as [opponent] ON [d].[OpponentUserId]=[opponent].[Id]
                       WHERE [d].[DuelId]=@duelId";

            return Connection.Query<DuelStarting>(sql, new { duelId }).SingleOrDefault();
        }

        /// <summary>
        /// Düello total scores günceller
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <param name="founderTotalScore">Kurucu puan</param>
        /// <param name="opponentTotalScore">Rakip puan</param>
        public void UpdateTotalScores(int duelId, byte founderTotalScore, byte opponentTotalScore)
        {
            if (duelId <= 0)
                return;

            string sql = @"UPDATE Duels SET FounderTotalScore=@founderTotalScore, OpponentTotalScore=@opponentTotalScore
                           WHERE DuelId=@duelId";

            int rowCount = Connection.Execute(sql, new { duelId, founderTotalScore, opponentTotalScore });

            if (rowCount <= 0)
                _logger.LogWarning($"Düello toplam skorlar güncellenemedi. Duel id: {duelId}");
        }

        #endregion Methods
    }
}