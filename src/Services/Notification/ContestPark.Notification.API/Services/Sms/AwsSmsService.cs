using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ContestPark.Notification.API.Services.Sms
{
    public class AwsSmsService : ISmsService
    {
        #region Private variables

        private readonly ILogger<AwsSmsService> _logger;
        private readonly NotificationSettings _notificationSettings;

        #endregion Private variables

        #region Methods

        public AwsSmsService(ILogger<AwsSmsService> logger,
                             IOptions<NotificationSettings> notificationSettings)
        {
            _logger = logger;
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
                StringValue = "Promotional",
                DataType = "String"
            });

            PublishResponse publishResponse = await notificationServiceClient.PublishAsync(publishRequest);

            if (publishResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                _logger.LogError("Sms gönderme başarısız oldu.", publishResponse.HttpStatusCode.ToString(), message);

            return publishResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        #endregion Methods
    }
}
