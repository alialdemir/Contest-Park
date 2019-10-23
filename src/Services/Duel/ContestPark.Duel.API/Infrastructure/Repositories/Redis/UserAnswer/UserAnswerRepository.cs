using ContestPark.Duel.API.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Redis.UserAnswer
{
    public class UserAnswerRepository : IUserAnswerRepository
    {
        #region Private Variables

        private readonly IDistributedCache _distributedCache;

        #endregion Private Variables

        #region Constructor

        public UserAnswerRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        #endregion Constructor

        #region Methods

        public List<UserAnswerModel> GetAnswers(int deuelId)
        {
            string key = GetKey(deuelId);

            string json = _distributedCache.GetString(key);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<List<UserAnswerModel>>(json);
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

            string data = JsonConvert.SerializeObject(userAnswers);

            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10));
            option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            _distributedCache.SetString(key, data);
        }

        /// <summary>
        /// Düello id göre cache item siler
        /// </summary>
        /// <param name="duelId">Düello id</param>
        public void Remove(int duelId)
        {
            string key = GetKey(duelId);

            _distributedCache.Remove(key);
        }

        private string GetKey(int duelId)
        {
            return $"DuelUserAnswer:{duelId}";
        }

        #endregion Methods
    }
}
