using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

namespace ContestPark.Admin.API.Infrastructure.Repositories.Question
{
    public class QuestionRepository : IQuestionRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Question> _questionRepository;

        #endregion Private Variables

        #region Constructor

        public QuestionRepository(IRepository<Tables.Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Soru ekle
        /// </summary>
        /// <param name="question">Soru bilgisi</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<int> Insert(Tables.Question question)
        {
            int? questionId = await _questionRepository.AddAsync(question);

            return questionId.Value;
        }

        #endregion Methods
    }
}
