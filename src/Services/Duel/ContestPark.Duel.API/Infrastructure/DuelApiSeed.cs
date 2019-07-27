using ContestPark.Core.Database.Infrastructure;
using ContestPark.Duel.API.Infrastructure.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure
{
    public class DuelApiSeed : ContextSeedBase<DuelApiSeed>
    {
        public async Task SeedAsync(IServiceProvider service, ILogger<DuelApiSeed> logger)
        {
            var policy = CreatePolicy();

            Service = service;
            Logger = logger;

            await policy.ExecuteAsync(async () =>
            {
                DateTime now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                await InsertDataAsync(new List<ContestDate>
                {
                    new ContestDate
                    {
                        StartDate = now,
                        FinishDate = now.AddDays(30)
                    }
                }); ; ;

                await InsertDataAsync(new List<ScoreRanking>
                {
                    new ScoreRanking
                    {
                        UserId = "1111-1111-1111-1111",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "10k",
                        DisplayTotalMoneyScore = "20k",
                        TotalGoldScore = 10000,
                        TotalMoneyScore = 20000,
                        ContestDateId = 1,
                    },
                    new ScoreRanking
                    {
                        UserId = "2222-2222-2222-2222",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "5k",
                        DisplayTotalMoneyScore = "5",
                        TotalGoldScore = 5000,
                        TotalMoneyScore = 5,
                        ContestDateId = 1,
                    },
                    new ScoreRanking
                    {
                        UserId = "3333-3333-3333-bot",
                        SubCategoryId = 1,
                        DisplayTotalGoldScore = "134",
                        DisplayTotalMoneyScore = "7k",
                        TotalGoldScore = 7004,
                        TotalMoneyScore = 1,
                        ContestDateId = 1,
                    },
                });

                await InsertDataAsync(new List<BalanceRanking>
                {
                    new BalanceRanking
                    {
                        UserId = "1111-1111-1111-1111",
                        ContestDateId = 1,
                        DisplayTotalGold = "10k",
                        TotalGold = 10000,
                        DisplayTotalMoney = "20 TL",
                        TotalMoney = 20.00M,
                    },
                    new BalanceRanking
                    {
                        UserId = "2222-2222-2222-2222",
                        ContestDateId = 1,
                        DisplayTotalGold = "5k",
                        TotalGold = 5000,
                        DisplayTotalMoney = "5 TL",
                        TotalMoney = 5.00M,
                    },
                    new BalanceRanking
                    {
                        UserId = "3333-3333-3333-bot",
                        ContestDateId = 1,
                        DisplayTotalGold = "134",
                        TotalGold = 134,
                        DisplayTotalMoney = "1 TL",
                        TotalMoney = 1.00M,
                    },
                });

                await InsertDataAsync(new List<Question>
                {
                    new Question
                     {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/ca.png",
                          IsActive = true,
                          SubCategoryId = 1,
                       },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/gb.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/jp.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/fr.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/br.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/it.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                    new Question
                    {
                          AnswerType = Enums.AnswerTypes.Text,
                          QuestionType = Enums.QuestionTypes.Image,
                          Link ="http://flags.fmcdn.net/data/flags/h80/ss.png",
                          IsActive = true,
                          SubCategoryId = 1,
                    },
                });

                await InsertDataAsync(new List<QuestionLocalized>
                {
                    new QuestionLocalized
                    {
                         Language = Core.Enums.Languages.Turkish,
                         Question ="Bu bayrak hangi ülkeye ait?"
                    },
                    new QuestionLocalized
                    {
                         Language = Core.Enums.Languages.English,
                         Question = "Which country does this flag belong to?"
                    }
                });

                await InsertDataAsync(new List<QuestionOfQuestionLocalized>
                {
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 1,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 1,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 2,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 2,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 3,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 3,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 4,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 4,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 5,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 5,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 6,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 6,
                         QuestionLocalizedId = 2,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 7,
                         QuestionLocalizedId = 1,
                    },
                    new QuestionOfQuestionLocalized
                    {
                         QuestionId = 7,
                         QuestionLocalizedId = 2,
                    },
                });

                await InsertDataAsync(new List<AnswerLocalized>
                {
                    new AnswerLocalized
                    {
                       QuestionId = 1,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Kanada",
                       Stylish1 = "Birleşik Krallık",
                       Stylish2 = "Japonya",
                       Stylish3 = "Fransa",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 1,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "Canada",
                       Stylish1 = "United Kingdom",
                       Stylish2 = "Japan",
                       Stylish3 = "France",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 2,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Birleşik Krallık",
                       Stylish1 = "Kanada",
                       Stylish2 = "Japonya",
                       Stylish3 = "Fransa",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 2,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "United Kingdom",
                       Stylish1 = "Canada",
                       Stylish2 = "Japan",
                       Stylish3 = "France",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 3,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Japonya",
                       Stylish1 = "Kanada",
                       Stylish2 = "Birleşik Krallık",
                       Stylish3 = "Fransa",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 3,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "Japan",
                       Stylish1 = "Canada",
                       Stylish2 = "United Kingdom",
                       Stylish3 = "France",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 4,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Fransa",
                       Stylish1 = "Kanada",
                       Stylish2 = "Birleşik Krallık",
                       Stylish3 = "Japonya",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 4,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "France",
                       Stylish1 = "Canada",
                       Stylish2 = "United Kingdom",
                       Stylish3 = "Japan",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 5,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Brezilya",
                       Stylish1 = "Kanada",
                       Stylish2 = "Birleşik Krallık",
                       Stylish3 = "Japonya",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 5,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "Brazil",
                       Stylish1 = "Canada",
                       Stylish2 = "United Kingdom",
                       Stylish3 = "Japan",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 6,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "İtalya",
                       Stylish1 = "Kanada",
                       Stylish2 = "Birleşik Krallık",
                       Stylish3 = "Japonya",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 6,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "Italy",
                       Stylish1 = "Canada",
                       Stylish2 = "United Kingdom",
                       Stylish3 = "Japan",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 7,
                       Language = Core.Enums.Languages.Turkish,
                       CorrectStylish = "Güney Sudan",
                       Stylish1 = "Kanada",
                       Stylish2 = "Birleşik Krallık",
                       Stylish3 = "Japonya",
                    },
                    new AnswerLocalized
                    {
                       QuestionId = 7,
                       Language = Core.Enums.Languages.English,
                       CorrectStylish = "South Sudan",
                       Stylish1 = "Canada",
                       Stylish2 = "United Kingdom",
                       Stylish3 = "Japan",
                    },
                });
            });
        }
    }
}
