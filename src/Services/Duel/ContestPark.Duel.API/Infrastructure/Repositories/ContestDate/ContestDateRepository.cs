using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Models;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ContestDate
{
    public class ContestDateRepository : IContestDateRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.ContestDate> _contestDateRepository;

        #endregion Private Variables

        #region Constructor

        public ContestDateRepository(IRepository<Tables.ContestDate> contestDateRepository)
        {
            _contestDateRepository = contestDateRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Aktif olan son yarışma bilgilerini veirir
        /// </summary>
        /// <returns>Aktif yarışma tarihi</returns>
        public ContestDateModel ActiveContestDate()
        {
            string sql = @"SELECT cd.ContestDateId, cd.FinishDate
                          FROM ContestDates cd
                          WHERE cd.StartDate <= CURRENT_TIMESTAMP()
                          ORDER BY cd.FinishDate DESC
                          LIMIT 1";

            return _contestDateRepository.QuerySingleOrDefault<ContestDateModel>(sql);
        }

        #endregion Methods
    }
}
