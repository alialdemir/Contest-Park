using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Question.Enums;
using ContestPark.Domain.Question.Model.Request;
using ContestPark.Domain.Question.Model.Response;
using ContestPark.Infrastructure.Question.Entities;
using ContestPark.Infrastructure.Question.Repositories.AskedQuestion;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.Repositories.Question
{
    public class QuestionRepository : DapperRepositoryBase<QuestionEntity>, IQuestionRepository
    {
        #region Private variables

        private readonly IAskedQuestionRepository _askedQuestion;

        #endregion Private variables

        #region Constructor

        public QuestionRepository(ISettingsBase settings,
                                 IAskedQuestionRepository askedQuestion) : base(settings)
        {
            _askedQuestion = askedQuestion ?? throw new ArgumentNullException(nameof(askedQuestion));
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Düello için rastgele soru getirir
        /// Kurucu ve rakibin dillerine göre soruları getirir örneğin kurucu türkçe rakip ingilizce oynarsa hem türkçe soru ve şıkları hemde ingilizce gelir
        /// Signalr tarafında cliente gönderirken dil bilgisine göre filtreleme yapıyoruz
        /// Bir kere sorulan soru o kategorilerdeki tüm sorular sorunmadan tekrar sorulmaz
        /// </summary>
        /// <param name="questionInfo">Soru bilgileri</param>
        /// <returns>Random Soru</returns>
        public async Task<Domain.Question.Model.Response.Question> GetQuestionAsync(QuestionInfo questionInfo)
        {
            Domain.Question.Model.Response.Question question = GetQuestionModel(questionInfo);

            // Eğer null gelirse o kategorideki tüm sorular sorulmuştur o zaman sorulan soruları temizledik
            if (question == null)
            {
                await _askedQuestion.DeleteAsync(questionInfo.FounderUserId, questionInfo.OpponentUserId, questionInfo.SubCategoryId);

                return await GetQuestionAsync(questionInfo);
            }

            return question;
        }

        #endregion Methods

        #region Private methods

        private Domain.Question.Model.Response.Question GetQuestionModel(QuestionInfo questionInfo)
        {
            string sql = @"DECLARE @QuestionInfoId int

                           SELECT top 1 @QuestionInfoId=[qi].[QuestionInfoId]
                           FROM QuestionInfos AS [qi]
                           LEFT JOIN [AskedQuestions] AS [aq] ON ([qi].[QuestionInfoId]<>[aq].[QuestionInfoId] AND [aq].[SubCategoryId]=[qi].[SubCategoryId]) AND [aq].UserId=@founderUserId
                           LEFT JOIN [AskedQuestions] AS [aq1] ON ([qi].[QuestionInfoId]<>[aq1].[QuestionInfoId] AND [aq1].[SubCategoryId]=[qi].[SubCategoryId]) AND [aq1].UserId=@opponentUserId
                           where [qi].[SubCategoryId]=@subCategoryId
                           ORDER BY NEWID()

                           SELECT
                           [qi].[QuestionInfoId],
                           [qa].[QuestionId],
                           [qi].[Link],
                           [qi].[AnswerType],
                           [qi].[QuestionType],
                           [ql].[Question],
                           [qa].[Answer],
                           [qa].[IsCorrect],
						   [qa].[LanguageId] as [LanguageId],
						   [ql].[LanguageId] as [Language]
                           FROM [QuestionAnswers] [qa]
                           INNER JOIN [QuestionInfos] [qi] on [qa].[QuestionInfoId]=[qi].[QuestionInfoId]
                           JOIN [QuestionLangs] AS [ql] ON ([qi].[QuestionId]=[ql].[QuestionId] AND [ql].LanguageId=@founderLanguage) or ([qi].[QuestionId]=[ql].[QuestionId] and [ql].LanguageId=@opponentLanguage)
                           where  ([qa].[QuestionInfoId]=@QuestionInfoId and [qa].LanguageId=@founderLanguage) or ([qa].[QuestionInfoId]=@QuestionInfoId AND [qa].LanguageId=@opponentLanguage)
                           ORDER BY NEWID()";

            Domain.Question.Model.Response.Question questionModel = new Domain.Question.Model.Response.Question();

            using (var reader = Connection.ExecuteReader(sql, new
            {
                founderUserId = questionInfo.FounderUserId,
                opponentUserId = questionInfo.OpponentUserId,
                subCategoryId = questionInfo.SubCategoryId,
                founderLanguage = questionInfo.FounderLanguage,
                opponentLanguage = questionInfo.OpponentLanguage
            }))
            {
                while (reader.Read())
                {
                    questionModel.QuestionInfoId = reader.GetInt32(0);
                    questionModel.QuestionId = reader.GetInt32(1);
                    questionModel.Link = reader.GetString(2);
                    questionModel.AnswerType = (AnswerTypes)reader.GetByte(3);
                    questionModel.QuestionType = (QuestionTypes)reader.GetByte(4);

                    string question = reader.GetString(5);
                    if (questionModel.Questions == null)
                        questionModel.Questions = new List<QuestionLang>();

                    if (questionModel.Questions.FindIndex(x => x.Question == question) == -1)
                    {
                        questionModel.Questions.Add(new QuestionLang()
                        {
                            Question = question,
                            Language = reader.GetByte(9)
                        });
                    }

                    string answer = reader.GetString(6);
                    if (questionModel.Answers == null)
                        questionModel.Answers = new List<Answer>();

                    if (questionModel.Answers.FindIndex(x => x.Answers == answer) == -1)
                    {
                        questionModel.Answers.Add(new Answer()
                        {
                            Answers = answer,
                            Language = reader.GetByte(8),
                            IsCorrect = reader.GetBoolean(7)
                        });
                    }
                }
            }

            return questionModel;
        }

        #endregion Private methods
    }
}