using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public class DuelRepository : IDuelRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Duel> _askedQuestionRepository;

        #endregion Private Variables

        #region Constructor

        public DuelRepository(IRepository<Tables.Duel> askedQuestionRepository)
        {
            _askedQuestionRepository = askedQuestionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duello ekle
        /// </summary>
        /// <param name="duel">Duello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<int?> Insert(Tables.Duel duel)
        {
            return _askedQuestionRepository.AddAndGetIdAsync(duel);
        }

        #endregion Methods
    }
}
