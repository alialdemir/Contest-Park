using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.QuestionOfQuestionLocalized
{
    public class QuestionOfQuestionLocalizedRepository : IQuestionOfQuestionLocalizedRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.QuestionOfQuestionLocalized> _questionOfQuestionLocalizedRepository;

        #endregion Private Variables

        #region Constructor

        public QuestionOfQuestionLocalizedRepository(IRepository<Tables.QuestionOfQuestionLocalized> questionOfQuestionLocalizedRepository)
        {
            _questionOfQuestionLocalizedRepository = questionOfQuestionLocalizedRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Soru dil ekle
        /// </summary>
        /// <param name="questionOfQuestion">Soru dil bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> Insert(Tables.QuestionOfQuestionLocalized questionOfQuestion)
        {
            int? questionOfQuestionLocalizedId = await _questionOfQuestionLocalizedRepository.AddAsync(questionOfQuestion);

            return questionOfQuestionLocalizedId.HasValue;
        }

        #endregion Methods
    }
}
