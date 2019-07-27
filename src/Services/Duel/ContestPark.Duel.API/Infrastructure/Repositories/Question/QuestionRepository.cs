using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.AskedQuestion;
using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Question
{
    public class QuestionRepository : IQuestionRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Question> _questionRepository;
        private readonly IAskedQuestionRepository _askedQuestionRepository;

        #endregion Private Variables

        #region Constructor

        public QuestionRepository(IRepository<Tables.Question> questionRepository,
                                  IAskedQuestionRepository askedQuestionRepository)
        {
            _questionRepository = questionRepository;
            _askedQuestionRepository = askedQuestionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duelloda sorulacak soruları getirir
        /// daha önce sorulmamış 7 soru döndürür
        ///  Kurucu ve rakip kullanıcının daha önce sorulmamış soruları seçilen dillere göre getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="founderUserId">Kurucu kullanıcı id</param>
        /// <param name="opponentUserId">Rakip kullanıcı id</param>
        /// <param name="founderLanguge">Kurucu dili</param>
        /// <param name="opponentLanguge">Rakip dili</param>
        /// <returns>Düelloda sorulacak sorular</returns>
        public async Task<IEnumerable<QuestionModel>> DuelQuestions(short subCategoryId, string founderUserId, string opponentUserId, Languages founderLanguge, Languages opponentLanguge)
        {
            if (string.IsNullOrEmpty(founderUserId) || string.IsNullOrEmpty(opponentUserId))
                return null;

            List<QuestionTableModel> duelQuestions = _questionRepository.QueryMultiple<QuestionTableModel>("SP_RandomQuestions", new
            {
                subCategoryId,
                founderUserId,
                opponentUserId,
                founderLanguge,
                opponentLanguge
            }, CommandType.StoredProcedure).ToList();

            // Eğer iki dil aynı ise sadece o dilde geleceği için 7 tane soru gelir farklı ise 14 soru gelir
            int questionCount = founderLanguge == opponentLanguge ? 7 : 14;
            if (duelQuestions.Count < questionCount)
            {
                bool isSuccess = await _askedQuestionRepository.Delete(subCategoryId, founderUserId, opponentUserId);
                if (!isSuccess)
                    return null;

                return await DuelQuestions(subCategoryId, founderUserId, opponentUserId, founderLanguge, opponentLanguge);
            }

            List<QuestionModel> questions = new List<QuestionModel>();

            foreach (var question in duelQuestions) // TODO: daha basit hale getir
            {
                QuestionModel q = questions.Where(x => x.QuestionId == question.QuestionId).FirstOrDefault();

                if (q == null)
                {
                    q = new QuestionModel
                    {
                        Link = question.Link,
                        AnswerType = question.AnswerType,
                        QuestionId = question.QuestionId,
                        QuestionType = question.QuestionType
                    };
                }

                q.Answers.Add(new AnswerModel
                {
                    Answers = question.CorrectStylish,
                    IsCorrect = true,
                    Language = question.Language
                });

                q.Answers.Add(new AnswerModel
                {
                    Answers = question.Stylish1,
                    IsCorrect = false,
                    Language = question.Language
                });

                q.Answers.Add(new AnswerModel
                {
                    Answers = question.Stylish2,
                    IsCorrect = false,
                    Language = question.Language
                });

                q.Answers.Add(new AnswerModel
                {
                    Answers = question.Stylish3,
                    IsCorrect = false,
                    Language = question.Language
                });

                q.Questions.Add(new QuestionLocalizedModel
                {
                    Language = question.Language,
                    Question = question.Question
                });

                if (!questions.Any(x => x.QuestionId == q.QuestionId))
                {
                    questions.Add(q);
                }
            }

            // Sorulan sorular eklendi
            _askedQuestionRepository.Insert(subCategoryId, questions.Select(x => x.QuestionId).ToArray(), founderUserId, opponentUserId);

            return questions;
        }

        #endregion Methods
    }
}
