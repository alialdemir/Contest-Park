using ContestPark.Core.DataSeed;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Duel.Enums;
using ContestPark.Infrastructure.Duel.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Infrastructure.Duel.DataSeed
{
    internal class DuelContextSeed : ContextSeedBase
    {
        public override async Task SeedAsync(ISettingsBase settings, ILogger logger)
        {
            var policy = CreatePolicy();

            await policy.ExecuteAsync(async () =>
            {
                ConnectionString = settings.ConnectionString;

                Logger = logger;
                SeedName = nameof(DuelContextSeed);

                await InsertDataAsync(GetDuels());

                await InsertDataAsync(GetDuelInfos());
            });
        }

        private IEnumerable<DuelEntity> GetDuels()
        {
            string witcherUser = "1111-1111-1111-1111";
            string demoUser = "2222-2222-2222-2222";
            string bootUser = "3333-3333-3333-bot";

            return new List<DuelEntity>
            {
                new DuelEntity
                {
                     FounderUserId = witcherUser,
                     OpponentUserId = demoUser,
                     Bet = 5000,
                     SubCategoryId = 1,
                     FounderTotalScore = 100,
                     OpponentTotalScore = 152,
                },
                new DuelEntity
                {
                     FounderUserId = witcherUser,
                     OpponentUserId = bootUser,
                     Bet = 10000,
                     SubCategoryId = 1,
                     FounderTotalScore = 50,
                     OpponentTotalScore = 38,
                },
                new DuelEntity
                {
                     FounderUserId = bootUser,
                     OpponentUserId = demoUser,
                     Bet = 15000,
                     SubCategoryId = 1,
                     FounderTotalScore = 80,
                     OpponentTotalScore = 250,
                }
            };
        }

        private IEnumerable<DuelInfoEntity> GetDuelInfos()// TODO: Question id elasticsearch  kısmı ile uyuşmalı
        {
            return new List<DuelInfoEntity>
            {
                new DuelInfoEntity
                {
                     DuelId = 1,
                     CorrectAnswer = Stylish.A,
                     FounderAnswer= Stylish.A,
                     OpponentAnswer =  Stylish.A,
                     FounderTime = 7,
                     OpponentTime = 6,
                     QuestionInfoId = 1,
                     FounderScore = 50,
                     OpponentScore = 76,
                },
                new DuelInfoEntity
                {
                     DuelId = 1,
                     CorrectAnswer = Stylish.C,
                     FounderAnswer= Stylish.B,
                     OpponentAnswer =  Stylish.D,
                     FounderTime = 10,
                     OpponentTime = 10,
                     QuestionInfoId = 2,
                     FounderScore = 50,
                     OpponentScore = 76,
                },

                new DuelInfoEntity
                {
                     DuelId = 2,
                     CorrectAnswer = Stylish.B,
                     FounderAnswer= Stylish.C,
                     OpponentAnswer =  Stylish.C,
                     FounderTime = 6,
                     OpponentTime = 8,
                     QuestionInfoId = 3,
                     FounderScore =  25,
                     OpponentScore = 19,
                },
                new DuelInfoEntity
                {
                     DuelId = 2,
                     CorrectAnswer = Stylish.D,
                     FounderAnswer= Stylish.A,
                     OpponentAnswer =  Stylish.D,
                     FounderTime= 2,
                     OpponentTime = 10,
                     QuestionInfoId = 4,
                     FounderScore = 25,
                     OpponentScore = 19,
                },

                new DuelInfoEntity
                {
                     DuelId = 3,
                     CorrectAnswer = Stylish.A,
                     FounderAnswer= Stylish.D,
                     OpponentAnswer =  Stylish.C,
                     FounderTime = 3,
                     OpponentTime = 9,
                     QuestionInfoId = 5,
                     FounderScore =  40,
                     OpponentScore = 125,
                },
                new DuelInfoEntity
                {
                     DuelId = 3,
                     CorrectAnswer = Stylish.B,
                     FounderAnswer= Stylish.A,
                     OpponentAnswer =  Stylish.C,
                     FounderTime = 10,
                     OpponentTime = 4,
                     QuestionInfoId = 6,
                     FounderScore = 40,
                     OpponentScore = 125,
                }
            };
        }
    }
}