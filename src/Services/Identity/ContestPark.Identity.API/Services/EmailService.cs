using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Services
{
    public class EmailService : IEmailService
    {
        #region Private variables

        private readonly ILogger<EmailService> _logger;
        private readonly string _sendGridApiKey;

        #endregion Private variables

        #region Constructor

        public EmailService(ILogger<EmailService> logger,
                            IConfiguration configuration)
        {
            _logger = logger;
            _sendGridApiKey = configuration["SendGridApiKey"];
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// info@contestpark.com üzerinden mail gönderir
        /// </summary>
        /// <param name="email">Alıcı email</param>
        /// <param name="subject">Mailinin konusu</param>
        /// <param name="message">Mesaj içeriği</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                _logger.LogWarning($"Mail gönderilirken bilgilerden biri boş geldi. Email: {email} subject: {subject} message: {message}");

                return false;
            }

            if (_sendGridApiKey == "Test")
            {
                _logger.LogInformation($"Test maili gönderildi. Email: {email} subject: {subject} message: {message}");
                return true;
            }

            try
            {
                // Html template
                var htmlTemplate = File.ReadAllText("EmailTheme.html").Replace("{Message}", message);

                var client = new SendGridClient(_sendGridApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("info@contestpark.com", "ContestPark"),
                    Subject = subject,
                    PlainTextContent = "text/plain",
                    HtmlContent = htmlTemplate
                };
                msg.AddTo(new EmailAddress(email));

                // Disable click tracking.
                // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
                msg.SetClickTracking(false, false);

                Response response = await client.SendEmailAsync(msg);

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Methods
    }
}