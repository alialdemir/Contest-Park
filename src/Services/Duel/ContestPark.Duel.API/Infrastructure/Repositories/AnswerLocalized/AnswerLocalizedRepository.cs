using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.AnswerLocalized
{
    public class AnswerLocalizedRepository : IAnswerLocalizedRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.AnswerLocalized> _answerLocalizedRepository;

        #endregion Private Variables

        #region Constructor

        public AnswerLocalizedRepository(IRepository<Tables.AnswerLocalized> answerLocalizedRepository)
        {
            _answerLocalizedRepository = answerLocalizedRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Soru dil ekle
        /// </summary>
        /// <param name="answerLocalized">Soru dil bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> Insert(Tables.AnswerLocalized answerLocalized)
        {
            int? questionLocalizedId = await _answerLocalizedRepository.AddAsync(answerLocalized);

            return questionLocalizedId.HasValue;
        }

        #endregion Methods
    }
}
