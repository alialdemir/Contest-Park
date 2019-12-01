using ContestPark.Core.Database.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.AnswerLocalized
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
        /// <param name="answerLocalizeds">Soru dil bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> Insert(List<Tables.AnswerLocalized> answerLocalizeds)
        {
            return _answerLocalizedRepository.AddRangeAsync(answerLocalizeds);
        }

        #endregion Methods
    }
}
