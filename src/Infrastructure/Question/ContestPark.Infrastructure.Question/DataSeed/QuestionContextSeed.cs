using ContestPark.Core.DataSeed;
using ContestPark.Core.Enums;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Question.Enums;
using ContestPark.Infrastructure.Question.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Question.DataSeed
{
    public class QuestionContextSeed : ContextSeedBase
    {
        public override async Task SeedAsync(ISettingsBase settings, ILogger logger)
        {
            var policy = CreatePolicy();

            await policy.ExecuteAsync(async () =>
            {
                ConnectionString = settings.ConnectionString;

                Logger = logger;
                SeedName = nameof(QuestionContextSeed);

                await InsertDataAsync(GetQuestions());

                await InsertDataAsync(GetQuestionInfos());

                await InsertDataAsync(GetAskedQuestions());

                await InsertDataAsync(GetQuestionAnswers());

                await InsertDataAsync(GetQuestionLangs());
            });
        }

        private IEnumerable<QuestionEntity> GetQuestions()
        {
            return new List<QuestionEntity>
            {
                new QuestionEntity()
            };
        }

        private IEnumerable<QuestionInfoEntity> GetQuestionInfos()
        {
            return new List<QuestionInfoEntity>
            {
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/ca.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/gb.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/jp.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/fr.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/br.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/it.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
                new QuestionInfoEntity{
                      AnswerType = AnswerTypes.Text,
                      QuestionType = QuestionTypes.Image,
                      Link ="http://flags.fmcdn.net/data/flags/h80/ss.png",
                      IsActive = true,
                      QuestionId = 1,
                      SubCategoryId = 1,
                },
            };
        }

        private IEnumerable<AskedQuestionEntity> GetAskedQuestions()
        {
            return new List<AskedQuestionEntity>
            {
                new AskedQuestionEntity
                {
                    UserId = "1111-1111-1111-1111",
                    SubCategoryId = 1,
                    QuestionInfoId = 1
                }
            };
        }

        private IEnumerable<QuestionAnswerEntity> GetQuestionAnswers()
        {
            return new List<QuestionAnswerEntity>
            {
                // Cevap 1
                new QuestionAnswerEntity
                {
                    Answer = "Kanada",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Birleşik Krallık",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japonya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Fransa",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Canada",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "United Kingdom",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japan",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },
                new QuestionAnswerEntity
                {
                    Answer = "France",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 1,
                },

                // Cevap 2
                new QuestionAnswerEntity
                {
                    Answer = "Kanada",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Birleşik Krallık",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japonya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Fransa",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Canada",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "United Kingdom",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japan",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },
                new QuestionAnswerEntity
                {
                    Answer = "France",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 2,
                },

                // Cevap 3
                new QuestionAnswerEntity
                {
                    Answer = "Kanada",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Birleşik Krallık",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japonya",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Fransa",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Canada",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "United Kingdom",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japan",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },
                new QuestionAnswerEntity
                {
                    Answer = "France",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 3,
                },

                // Cevap 4
                new QuestionAnswerEntity
                {
                    Answer = "Kanada",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Birleşik Krallık",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japonya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Fransa",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Canada",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "United Kingdom",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Japan",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },
                new QuestionAnswerEntity
                {
                    Answer = "France",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 4,
                },

                // Cevap 5
                new QuestionAnswerEntity
                {
                    Answer = "Brezilya",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "İtalya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Güney Sudan",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Brazil",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Italy",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "South Sudan",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibia",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 5,
                },

                // Cevap 6
                new QuestionAnswerEntity
                {
                    Answer = "Brezilya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "İtalya",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Güney Sudan",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Brazil",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Italy",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "South Sudan",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibia",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 6,
                },

                // Cevap 7
                new QuestionAnswerEntity
                {
                    Answer = "Brezilya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "İtalya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Güney Sudan",
                    IsCorrect = true,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibya",
                    IsCorrect = false,
                    LanguageId = Languages.Turkish,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },

                new QuestionAnswerEntity
                {
                    Answer = "Brazil",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Italy",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "South Sudan",
                    IsCorrect = true,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
                new QuestionAnswerEntity
                {
                    Answer = "Namibia",
                    IsCorrect = false,
                    LanguageId = Languages.English,
                    QuestionId = 1,
                    QuestionInfoId = 7,
                },
            };
        }

        private IEnumerable<QuestionLangEntity> GetQuestionLangs()
        {
            return new List<QuestionLangEntity>
            {
                new QuestionLangEntity
                {
                     LanguageId = Languages.Turkish,
                     QuestionId = 1,
                     Question = "Bu bayrak hangi ülkeye ait?",
                },
                new QuestionLangEntity
                {
                     LanguageId = Languages.English,
                     QuestionId = 1,
                     Question = "Which country does this flag belong to?",
                },
            };
        }
    }
}