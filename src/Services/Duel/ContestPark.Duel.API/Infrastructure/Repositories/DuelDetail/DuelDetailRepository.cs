using ContestPark.Core.Database.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.DuelDetail
{
    public class DuelDetailRepository : IDuelDetailRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.DuelDetail> _duelDetailrepository;

        #endregion Private Variables

        #region Constructor

        public DuelDetailRepository(IRepository<Tables.DuelDetail> duelDetailrepository)
        {
            _duelDetailrepository = duelDetailrepository;
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
