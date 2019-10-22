using ContestPark.Core.Dapper.Abctract;
using ContestPark.Duel.API.Models;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer
{
    public class UserAnswerRepository : Disposable, IUserAnswerRepository
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

        public List<UserAnswerModel> GetAnswers(int deuelId)
        {
            string key = GetKey(deuelId);

            if (!_redisClient.ContainsKey(key))
                return null;

            return _redisClient.Get<List<UserAnswerModel>>(key);
        }

        public void Add(UserAnswerModel userAnswer)
        {
            AddRangeAsync(new List<UserAnswerModel>
            {
                userAnswer
            });
        }

        public void AddRangeAsync(List<UserAnswerModel> userAnswers)
        {
            if (userAnswers == null || userAnswers.Count == 0)
                return;

            int duelId = userAnswers.FirstOrDefault().DuelId;

            string key = GetKey(duelId);

            _redisClient.Set<List<UserAnswerModel>>(key, userAnswers, expiresAt: DateTime.Now.AddMinutes(10));// 10 dk sonra redis üzerinden otomatik siler
        }

        /// <summary>
        /// Düello id göre cache item siler
        /// </summary>
        /// <param name="duelId">Düello id</param>
        public void Remove(int duelId)
        {
            string key = GetKey(duelId);
            if (_redisClient.ContainsKey(key))
            {
                _redisClient.Remove(key);
            }
        }

        private string GetKey(int duelId)
        {
            return $"DuelUserAnswer:{duelId}";
        }

        public override void DisposeCore()
        {
            base.DisposeCore();

            if (_redisClient != null)
                _redisClient.Dispose();
        }

        #endregion Methods
    }
}
