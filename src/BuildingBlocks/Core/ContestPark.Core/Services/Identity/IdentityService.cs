﻿using ContestPark.Core.Models;
using ContestPark.Core.Services.RequestProvider;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
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

        private readonly string baseUrl = "";
        private const string redisKey = "UserInfos";
        private readonly IRedisClient _redisClient;

        #endregion Private Variables

        #region Constructor

        public IdentityService(IRequestProvider requestProvider,
                               IConfiguration configuration,
                               IRedisClient redisClient)
        {
            _requestProvider = requestProvider ?? throw new ArgumentNullException(nameof(requestProvider));
            _redisClient = redisClient;
            string identityUrl = configuration["identityUrl"] ?? throw new ArgumentNullException(nameof(baseUrl));
            baseUrl = identityUrl + "/api/v1/account";
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı idlerine ait adsoyad, kullanıcı adı, profil resmi, user id bilgilerini döner
        /// </summary>
        /// <param name="userIds">Kullanıcı idleri</param>
        /// <param name="includeCoverPicturePath">
        /// Eğer true olursa kapak resminide döndürür false ise kapak resmini döndürmez
        /// Düello başlama ekranında kapat resmi lazım oldu o yüzden kullandık
        /// </param>
        /// <returns>Kullanıcı bilgilei</returns>
        public async Task<IEnumerable<UserModel>> GetUserInfosAsync(IEnumerable<string> userIds, bool includeCoverPicturePath = false)
        {
            List<UserModel> users = new List<UserModel>();
            foreach (string userId in userIds)
            {
                // keyleri alt kategori id, bahis miktarı ve bakiye tipine göre filtreledik
                string key = $"{redisKey}:{userId}*";

                var items = _redisClient.ScanAllKeys(key).ToList();
                if (items != null || items.Count != 0)
                {
                    users.AddRange(_redisClient.GetValues<UserModel>(items));
                }
            }

            var notFoundUserIds = userIds.Where(u => !users.Any(x => x.UserId == u)).AsEnumerable();
            if (notFoundUserIds.Count() > 0) // users listesinde yani redisde olmayan kullanıcıları gidip identity serviceden alıp redise ekleyip return ediyoruz
            {
                IEnumerable<UserModel> serviceUsers = await _requestProvider.PostAsync<IEnumerable<UserModel>>($"{baseUrl}/UserInfos?includeCoverPicturePath={includeCoverPicturePath}", userIds);

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

            foreach (UserModel user in users)
            {
                _redisClient.Set<UserModel>(GetKey(user.UserId, user.UserName), user, expiresAt: DateTime.Now.AddMinutes(30));// 30 dakkika sonra redis üzerinden otomatik siler
            }
        }

        private string GetKey(string userId, string userName)
        {
            return $"{redisKey}:{userId}:{userName}";
        }

        /// <summary>
        /// Kullannıcı adına ait user name döndürür
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        /// <returns>Kullanıcı id</returns>
        public async Task<UserIdModel> GetUserIdByUserName(string userName)
        {
            // keyleri alt kategori id, bahis miktarı ve bakiye tipine göre filtreledik
            string key = $"*:{userName}";

            var items = _redisClient.ScanAllKeys(key).ToList();
            if (items == null || items.Count == 0)
                return null;

            // Redis keyleri DuelUserModele çevirdik
            List<UserModel> users = _redisClient.GetValues<UserModel>(items);
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
