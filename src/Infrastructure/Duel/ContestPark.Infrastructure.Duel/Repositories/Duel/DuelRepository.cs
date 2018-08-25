using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Duel.Model.Response;
using ContestPark.Infrastructure.Duel.Entities;
using Dapper;
using System.Linq;

namespace ContestPark.Infrastructure.Duel.Repositories.Duel
{
    internal class DuelRepository : DapperRepositoryBase<DuelEntity>, IDuelRepository
    {
        #region Constructor

        public DuelRepository(ISettingsBase settingsBase) : base(settingsBase)
        {
        }

        #endregion Constructor

        #region Methods

        public DuelStarting GetDuelStarting(int duelId)
        {
            if (duelId < 0)
            {
                return null;
            }

            var sql = @"SELECT TOP 1
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

        #endregion Methods
    }
}