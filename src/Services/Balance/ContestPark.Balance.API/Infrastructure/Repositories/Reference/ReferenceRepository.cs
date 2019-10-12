using ContestPark.Balance.API.Models;
using ContestPark.Core.Database.Interfaces;

namespace ContestPark.Balance.API.Infrastructure.Repositories.Reference
{
    public class ReferenceRepository : IReferenceRepository
    {
        #region Private variables

        private readonly IRepository<Tables.Reference> _referenceRepository;

        #endregion Private variables

        #region Constructor

        public ReferenceRepository(IRepository<Tables.Reference> referenceRepository)
        {
            _referenceRepository = referenceRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Parametreden gelen referans kodu geçerli mi kontrol eder
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Referans kodu geçerli ise true değilse false döner</returns>
        public ReferenceModel IsCodeActive(string code, string userId)
        {
            return _referenceRepository.QuerySingleOrDefault<ReferenceModel>("SP_IsCodeActive", new
            {
                code,
                userId
            }, System.Data.CommandType.StoredProcedure);
        }

        #endregion Methods
    }
}
