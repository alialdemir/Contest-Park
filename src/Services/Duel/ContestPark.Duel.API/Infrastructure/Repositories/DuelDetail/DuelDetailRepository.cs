using ContestPark.Core.Database.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail
{
    public class DuelDetailRepository : IDuelDetailRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.DuelDetail> _duelDetailrepository;
        private readonly ILogger<DuelDetailRepository> _logger;

        #endregion Private Variables

        #region Constructor

        public DuelDetailRepository(IRepository<Tables.DuelDetail> duelDetailrepository,
                                    ILogger<DuelDetailRepository> logger)
        {
            _duelDetailrepository = duelDetailrepository;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düello detayları ekler
        /// </summary>
        /// <param name="duelDetails">Düello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> AddRangeAsync(IEnumerable<Tables.DuelDetail> duelDetails)
        {
            return _duelDetailrepository.AddRangeAsync(duelDetails);
        }

        #endregion Methods
    }
}
