using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using ContestPark.Core.Dapper.Abctract;
using ContestPark.Notification.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack.Redis;
using System;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.Sms
{
    public class AwsSmsService : Disposable, ISmsService
    {
        #region Private variables

        private readonly ILogger<AwsSmsService> _logger;
        private readonly IRedisClient _redisClient;
        private readonly NotificationSettings _notificationSettings;

        #endregion Private variables

        #region Methods

        public AwsSmsService(ILogger<AwsSmsService> logger,
                             IRedisClient redisClient,
                             IOptions<NotificationSettings> notificationSettings)
        {
            _logger = logger;
            _redisClient = redisClient;
            _notificationSettings = notificationSettings.Value;
        }

        #endregion Methods

        #region Methods

        /// <summary>
        /// Sms gönderme servisi
        /// </summary>
        /// <param name="message">Sms mesaajı</param>
        /// <param name="phoneNumber">Telefon numarası</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> SendSms(string message, string phoneNumber)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(phoneNumber))
                return false;

            string awsKeyId = _notificationSettings.AwsSmsKeyId;
            string awsSecret = _notificationSettings.AwsSmsSecret;

            var awsCredentials = new BasicAWSCredentials(awsKeyId, awsSecret);
            AmazonSimpleNotificationServiceClient notificationServiceClient = new AmazonSimpleNotificationServiceClient(awsCredentials, Amazon.RegionEndpoint.EUCentral1);
            PublishRequest publishRequest = new PublishRequest()
            {
                Message = message,
                PhoneNumber = phoneNumber
            };

            publishRequest.MessageAttributes.Add("AWS.SNS.SMS.SenderID", new MessageAttributeValue
            {
                StringValue = "ContestPark",
                DataType = "String"
            });
            publishRequest.MessageAttributes.Add("AWS.SNS.SMS.SMSType", new MessageAttributeValue
            {
                StringValue = "Transactional",
                DataType = "String"
            });

            PublishResponse publishResponse = await notificationServiceClient.PublishAsync(publishRequest);

            if (publishResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                _logger.LogError("Sms gönderme başarısız oldu.", publishResponse.HttpStatusCode.ToString(), message);

            return publishResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Sms kodunu döndürür
        /// </summary>
        /// <param name="key">User id</param>
        /// <returns>Sms kodu</returns>
        public SmsRedisModel GetSmsCode(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                var items = _redisClient.GetAllKeys();
                if (items == null || items.Count == 0)
                    return null;

                var code = _redisClient.Get<SmsRedisModel>(key);

                return code;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms ile gönderilen kod alma işleminde hata oluştu.");

                return null;
            }
        }

        /// <summary>
        /// Sms kodu redise ekler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="code">Sms kodu</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public bool Insert(SmsRedisModel sms)
        {
            try
            {
                string key = sms.PhoneNumber;

                if (_redisClient.ContainsKey(key))// Eğer redisde başka sms kodu kayıtlı ise önce onu siliyoruz
                    Delete(key);

                return _redisClient.Set<SmsRedisModel>(key, sms, expiresIn: TimeSpan.FromMinutes(5));// 5 dk sonra redis üzerinden otomatik siler
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms ile gönderilen kod ekleme işleminde hata oluştu.");

                return false;
            }
        }

        /// <summary>
        /// Redis üzerindeki sms kodunu siler
        /// </summary>
        /// <param name="key">Kullanıcı id</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public bool Delete(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                if (!_redisClient.ContainsKey(key))
                    return false;

                return _redisClient.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sms ile gönderilen kod silme işleminde hata oluştu.");

                return false;
            }
        }

        #endregion Methods

        #region Private methds

        public override void DisposeCore()
        {
            base.DisposeCore();

            if (_redisClient != null)
                _redisClient.Dispose();
        }

        #endregion Private methds
    }
}
