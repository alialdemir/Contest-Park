using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Models;
using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Yeni yarışma tarihi ekle
        /// </summary>
        /// <param name="startedDate">Yarışma başlangıç tarihi</param>
        /// <param name="finishDate">Yarışma bitiş tarihi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> AddAsync(DateTime startedDate, DateTime finishDate)
        {
            int? id = await _contestDateRepository.AddAsync(new Tables.ContestDate
            {
                StartDate = startedDate,
                FinishDate = finishDate
            });

            return id.HasValue && id.Value > 0;
        }

        #endregion Methods
    }
}
