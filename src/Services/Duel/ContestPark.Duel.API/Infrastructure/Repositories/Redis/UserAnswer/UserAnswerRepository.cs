using ContestPark.Duel.API.Models;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer
{
    public class UserAnswerRepository : IUserAnswerRepository
    {
        #region Private Variables

        private readonly IRedisClient _redisClient;

        #endregion Private Variables

        #region Constructor

        public UserAnswerRepository(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        #endregion Constructor

        #region Methods

        public List<UserAnswerModel> GetAnswers(UserAnswerModel duelUser)
        {
            string key = GetKey(duelUser.DuelId);

            return _redisClient.Get<List<UserAnswerModel>>(key);
        }

        public void Insert(UserAnswerModel userAnswer)
        {
            string key = GetKey(userAnswer.DuelId);

            var userAnswers = GetAnswers(userAnswer);

            if (userAnswers == null)
                userAnswers = new List<UserAnswerModel>();

            userAnswers.Add(userAnswer);

            _redisClient.Set<List<UserAnswerModel>>(key, userAnswers, expiresAt: DateTime.Now.AddMinutes(3));// 3 dk sonra redis üzerinden otomatik siler
        }

        private string GetKey(int duelId)
        {
            return $"DuelUserAnswer:{duelId}";
        }

        #endregion Methods
    }
}
