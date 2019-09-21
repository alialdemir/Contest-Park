using ContestPark.Duel.API.Infrastructure.Repositories.AnswerLocalized;
using ContestPark.Duel.API.Infrastructure.Repositories.Question;
using ContestPark.Duel.API.Infrastructure.Repositories.QuestionLocalized;
using ContestPark.Duel.API.Infrastructure.Repositories.QuestionOfQuestionLocalized;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.Picture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class QuestionController : Core.Controllers.ControllerBase
    {
        #region Private Variables

        private readonly IQuestionLocalizedRepository _questionLocalizedRepository;
        private readonly IAnswerLocalizedRepository _answerLocalizedRepository;
        private readonly IQuestionOfQuestionLocalizedRepository _questionOfQuestionLocalizedRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IQuestionRepository _questionRepository;

        #endregion Private Variables

        #region Constructor

        public QuestionController(ILogger<QuestionController> logger,
                                  IQuestionLocalizedRepository questionLocalizedRepository,
                                  IAnswerLocalizedRepository answerLocalizedRepository,
                                  IQuestionOfQuestionLocalizedRepository questionOfQuestionLocalizedRepository,
                                  IFileUploadService fileUploadService,
                                  IQuestionRepository questionRepository) : base(logger)
        {
            _questionLocalizedRepository = questionLocalizedRepository;
            _answerLocalizedRepository = answerLocalizedRepository;
            _questionOfQuestionLocalizedRepository = questionOfQuestionLocalizedRepository;
            _fileUploadService = fileUploadService;
            _questionRepository = questionRepository;
        }

        #endregion Constructor

        #region Methods

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult AddQuestion([FromBody]List<QuestionSaveModel> questions)// Oyunucunun karşısına rakip ekler
        {
            Task.Factory.StartNew(async () =>
            {
                Logger.LogInformation("Yeni sorular ekleniyor...");

                foreach (var item in questions)
                {
                    foreach (var question in item.Questions)
                    {
                        string fileUrl = string.Empty;

                        if (!string.IsNullOrEmpty(question.Link))
                        {
                            fileUrl = await _fileUploadService.UploadFileToStorageAsync(question.Link, question.SubCategoryId);
                            if (string.IsNullOrEmpty(fileUrl))
                            {
                                Logger.LogError("Soru yükleme işlemi başarısız oldu");
                                continue;
                            }
                        }

                        int questionId = await _questionRepository.Insert(new Infrastructure.Tables.Question
                        {
                            Link = fileUrl,
                            SubCategoryId = question.SubCategoryId,
                            AnswerType = question.AnswerTypes,
                            QuestionType = question.QuestionType,
                        });

                        foreach (var ql in item.QuestionLocalized)
                        {
                            int questionLocalizedId = _questionLocalizedRepository.IsQuestionRegistry(ql.Question);
                            if (questionLocalizedId == 0)
                            {
                                questionLocalizedId = await _questionLocalizedRepository.Insert(new Infrastructure.Tables.QuestionLocalized
                                {
                                    Language = ql.Language,
                                    Question = ql.Question,
                                });
                            }

                            await _questionOfQuestionLocalizedRepository.Insert(new Infrastructure.Tables.QuestionOfQuestionLocalized
                            {
                                QuestionId = questionId,
                                QuestionLocalizedId = questionLocalizedId
                            });
                        }

                        foreach (var answer in item.Answers)
                        {
                            await _answerLocalizedRepository.Insert(new Infrastructure.Tables.AnswerLocalized
                            {
                                QuestionId = questionId,
                                Language = answer.Language,
                                CorrectStylish = answer.CorrectStylish,
                                Stylish1 = answer.Stylish1,
                                Stylish2 = answer.Stylish2,
                                Stylish3 = answer.Stylish3,
                            });
                        }
                    }
                }

                Logger.LogInformation($"Yeni sorular ekrandi. Eklenen soru sayısı {questions.Count}");
            });

            return Ok();
        }

        #endregion Methods
    }
}
