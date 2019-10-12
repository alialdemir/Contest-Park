using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Balance.API.Infrastructure.Repositories.ReferenceCode
{
    public class ReferenceCodeRepostory : IReferenceCodeRepostory
    {
        #region Private variables

        private readonly IRepository<Tables.ReferenceCode> _referenceCodeRepository;

        #endregion Private variables

        #region Constructor

        public ReferenceCodeRepostory(IRepository<Tables.ReferenceCode> referenceCodeRepository)
        {
            _referenceCodeRepository = referenceCodeRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Referans kodu ile yapılan işlemi ekle
        /// </summary>
        /// <param name="referenceCode">Referans kod</param>
        public async Task<bool> Insert(string code, string referenceUserId, string newUserId)
        {
            int? referenceCodeId = await _referenceCodeRepository.AddAsync(new Tables.ReferenceCode
            {
                NewUserId = newUserId,
                ReferenceUserId = referenceUserId,
                Code = code
            });

            return referenceCodeId != null && referenceCodeId > 0;
        }

        #endregion Methods
    }
}
