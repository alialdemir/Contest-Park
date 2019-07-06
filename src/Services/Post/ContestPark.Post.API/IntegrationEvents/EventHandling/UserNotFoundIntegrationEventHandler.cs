using ContestPark.Core.Database.Interfaces;
using ContestPark.EventBus.Abstractions;
using ContestPark.Post.API.Infrastructure.Documents;
using ContestPark.Post.API.IntegrationEvents.Events;
using ContestPark.Post.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContestPark.Post.API.IntegrationEvents.EventHandling
{
    public class UserNotFoundIntegrationEventHandler : IIntegrationEventHandler<UserNotFoundIntegrationEvent>
    {
        private readonly IRepository<User> _userRepository;
        private readonly PostSettings _postSettings;
        private readonly ILogger<UserNotFoundIntegrationEventHandler> _logger;

        public UserNotFoundIntegrationEventHandler(IRepository<User> userRepository,
                                                   IOptions<PostSettings> postSettings,
                                                   ILogger<UserNotFoundIntegrationEventHandler> logger)
        {
            _postSettings = postSettings.Value;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Follow veritabanında bulunamayan kullanıcıları identity api'ye istek atıp kullanıcı bilgileri alır
        /// kendi veritabanına ekler eğer işlem başarısız olursa birer dakkika arayla beş kere tekrarlar
        /// </summary>
        public Task Handle(UserNotFoundIntegrationEvent @event)
        {
            GetRetryPolicy()
                .ExecuteAsync(async () =>
                        {
                            using (var httpClient = new HttpClient() { BaseAddress = new Uri(_postSettings.identityUrl) })
                            {
                                httpClient.DefaultRequestHeaders.Add("ClientKey", _postSettings.ClientKey);
                                httpClient.DefaultRequestHeaders.Add("ServiceName", _postSettings.Audience);

                                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/account/UserInfos");

                                string content = JsonConvert.SerializeObject(@event.NotFoundUserIds);
                                httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");

                                HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);

                                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                {
                                    _logger.LogCritical($"Identity üzerinden istendi fakat status code:  {(int)response.StatusCode}", @event.NotFoundUserIds);

                                    throw new ArgumentException("Identity api üzerinden kullanıcı bilgileri çekilemedi.");
                                }

                                string responseJSON = await response.Content.ReadAsStringAsync();
                                List<UserNotFoundModel> users = JsonConvert.DeserializeObject<List<UserNotFoundModel>>(responseJSON);
                                if (users == null || users.Count == 0)
                                {
                                    _logger.LogCritical($"Follow api de kullanıcı tablosunda bulunamayan kayıtlar identity üzerinden istendi fakat liste boş geldi", @event.NotFoundUserIds);

                                    throw new ArgumentException("Identiy api üzerinden kullanıcı bilgileri alınamadıi.");
                                }

                                bool isSucess = await _userRepository.AddRangeAsync(users.Select(u => new User
                                {
                                    Id = u.UserId,
                                    FullName = u.FullName,
                                    ProfilePicturePath = u.ProfilePicturePath,
                                    UserName = u.UserName
                                }).AsEnumerable());

                                if (!isSucess)
                                    throw new ArgumentException("Bulunamayan kullanıcılar user tablosuna kayıt edilemedi.");
                            }
                        });

            return Task.CompletedTask;
        }

        private static AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                .Handle<ArgumentException>()
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                    });
        }
    }
}
