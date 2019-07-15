using ContestPark.Core.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Core.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        #region Private Variables

        private readonly IRequestProvider _requestProvider;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        private readonly string baseUrl = "";
        private readonly string redisKey = "UserInfos";

        #endregion Private Variables

        #region Constructor

        public IdentityService(IRequestProvider requestProvider,
                               IConfiguration configuration,
                               ConnectionMultiplexer redis)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));

            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _database = redis.GetDatabase();

            string identityUrl = configuration["identityUrl"] ?? throw new ArgumentNullException(nameof(baseUrl));
            baseUrl = identityUrl + "/api/v1/account";
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı idlerine ait adsoyad, kullanıcı adı, profil resmi, user id bilgilerini döner
        /// </summary>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <returns>Kullanıcı bilgilei</returns>
        public async Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds)
        {
            List<UserModel> users = await GetUsersAsync();

            var notFoundUserIds = userIds.Where(u => !users.Any(x => x.UserId == u)).AsEnumerable();
            if (notFoundUserIds.Count() > 0) // users listesinde yani redisde olmayan kullanıcıları gidip identity serviceden alıp redise ekleyip return ediyoruz
            {
                IEnumerable<UserModel> serviceUsers = await _requestProvider.PostAsync<IEnumerable<UserModel>>($"{baseUrl}/UserInfos", userIds);

                if (serviceUsers != null || serviceUsers.Count() > 0)// Eğer identity servisten yeni kullanıcılar gelirse onlarıda result listesine ve redis'e ekledik
                {
                    users.AddRange(serviceUsers);

                    _ = Task.Factory.StartNew(() =>
                      {
                          SetUsers(users.Distinct());
                      });
                }
            }

            List<UserModel> result = users
                                        .Distinct()// TODO: redise eklerken zaten varsa eklememeli
                                        .Where(x => userIds.Any(user => user == x.UserId))
                                        .ToList();

            return result;
        }

        /// <summary>
        /// Redise kullanıcı listesi set eder
        /// 30 dakkika işlem yapılmazsa siler
        /// 60 dakkika da bir tümünü siler
        /// </summary>
        /// <param name="users">Redise eklenecek kullanıcılar</param>
        private void SetUsers(IEnumerable<UserModel> users)
        {
            if (users == null || users.ToList().Count == 0)
                return;

            string userJson = JsonConvert.SerializeObject(users);

            _database.StringSetAsync(redisKey, userJson, expiry: TimeSpan.FromMinutes(30), when: When.NotExists);
        }

        /// <summary>
        /// Redisdeki tüm kullanıcı listesini getirir
        /// </summary>
        /// <returns>Kullanıcı listesi</returns>
        private async Task<List<UserModel>> GetUsersAsync()
        {
            string users = await _database.StringGetAsync(redisKey);
            if (string.IsNullOrEmpty(users))
                return new List<UserModel>();

            List<UserModel> userModels = JsonConvert.DeserializeObject<List<UserModel>>(users);

            return userModels;
        }

        /// <summary>
        /// Kullannıcı adına ait user name döndürür
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        /// <returns>Kullanıcı id</returns>
        public async Task<UserIdModel> GetUserIdByUserName(string userName)
        {
            List<UserModel> users = await GetUsersAsync();
            if (users != null && users.Count != 0)
            {
                string userId = users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower()).UserId;
                if (!string.IsNullOrEmpty(userName))
                    return new UserIdModel { UserId = userId };
            }

            return await _requestProvider.GetAsync<UserIdModel>($"{baseUrl}/UserId?userName={userName}");
        }

        #endregion Methods
    }
}
