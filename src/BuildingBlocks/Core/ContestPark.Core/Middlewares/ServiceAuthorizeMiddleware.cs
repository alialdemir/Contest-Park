using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContestPark.Core.Middlewares
{
    /// <summary>
    /// Kendi servislerimiz bir birleri ile http üzerinden haberleşbilsin diye bu middleware yazıldı
    /// servis http request'tin header kısmında ClientKey yollayarak login olabilir
    /// login oldukları kullanıcı benim witcherfearless kullanıcınım
    /// servisler kendi aralarında http isteğinde haberleşirken user id login olan kullanıcı olarak kullanılmamalı
    /// </summary>
    public class ServiceAuthorizeMiddleware
    {
        #region Private Variables

        public const string USER_ID = "1111-1111-1111-1111";

        private readonly RequestDelegate _next;

        private readonly ILogger<ServiceAuthorizeMiddleware> _logger;
        private readonly IConfiguration _configuration;

        #endregion Private Variables

        #region Constructor

        public ServiceAuthorizeMiddleware(RequestDelegate rd,
                                       ILogger<ServiceAuthorizeMiddleware> logger,
                                       IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _next = rd;
        }

        #endregion Constructor

        #region Method

        /// <summary>
        /// Eğer ServiceName ve ClientKey geçerli ise o kullanıcıyı 1111-1111-1111-1111 kullanıcısı ile login olmuş kabul eder
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string clientKey = httpContext.Request.Headers["ClientKey"];
            string host = httpContext.Request.Host.ToString();

            /*
             Bizim kendi servislerimiz birbirlerine istek atabilmesi için host contestpark.
             ile başlaması lazım sonuda api ile bitmesi lazım ve client key env dosyasından gelen ile aynı olmalı
             */
            if (host.StartsWith("contestpark.") && host.EndsWith(".api") && !string.IsNullOrEmpty(clientKey))// TODO: burayı daha güvenli hale getirin
            {
                if (!clientKey.Equals(_configuration["ClientKey"]))
                {
                    _logger.LogCritical($"CRITICAL: Tanınmayan bir yerden bizim servisimizmiş gibi login olmaya çalışıyor. serviceName:{host} clientKey:{clientKey}");

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    return;
                }

                _logger.LogInformation($"{host} isimli servisten istek alındı.");

                var identity = new ClaimsIdentity("cookies");

                identity.AddClaim(new Claim("sub", USER_ID));
                identity.AddClaim(new Claim("unique_name", USER_ID));
                identity.AddClaim(new Claim("IsService", "true"));// Bizim servisimiz olduğunu policy anlaması için bu değeri atadık

                httpContext.User.AddIdentity(identity);
            }

            await _next.Invoke(httpContext);
        }

        #endregion Method
    }
}