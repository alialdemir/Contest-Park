using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.QuestionLocalized
{
    public class QuestionLocalizedRepository : IQuestionLocalizedRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.QuestionLocalized> _questionLocalizedRepository;

        #endregion Private Variables

        #region Constructor

        public QuestionLocalizedRepository(IRepository<Tables.QuestionLocalized> questionLocalizedRepository)
        {
            _questionLocalizedRepository = questionLocalizedRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Soru dil ekle
        /// </summary>
        /// <param name="questionLocalized">Soru dil bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<int> Insert(Tables.QuestionLocalized questionLocalized)
        {
            int? questionLocalizedId = await _questionLocalizedRepository.AddAsync(questionLocalized);

            return questionLocalizedId.Value;
        }

        /// <summary>
        /// Benzer soru ekli ise QuestionLocalizedId döner
        /// </summary>
        /// <param name="question">Soru cümlesi</param>
        /// <returns>QuestionLocalizedId</returns>
        public int IsQuestionRegistry(string question)
        {
            string sql = @"SELECT ql.QuestionLocalizedId FROM QuestionLocalizeds ql
WHERE ql.Question = @question
LIMIT 1";
            return _questionLocalizedRepository.QuerySingleOrDefault<int>(sql, new
            {
                question
            });
        }

        #endregion Methods
    }
}
