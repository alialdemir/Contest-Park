using ContestPark.Identity.API.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Middlewares
{
    /// <summary>
    /// Kendi servislerimiz bir birleri ile http üzerinden haberleşbilsin diye bu middleware yazıldı
    /// servis http reqesutin header kısmında ServiceName ve ClientKey yollayarak identity server üzerinde login olabilir
    /// login oldukları kullanıcı benim witcherfearless kullanıcınım
    /// servisler kendi aralarında http isteğinde haberleşirken user id login olan kullanıcı olarak kullanılmamalı
    /// </summary>
    public class ServiceAuthorizeMiddleware
    {
        public const string USER_ID = "1111-1111-1111-1111";

        private readonly RequestDelegate _next;

        private readonly ILogger<ServiceAuthorizeMiddleware> _logger;
        private readonly IOptions<IdentitySettings> _identitySettings;

        public ServiceAuthorizeMiddleware(RequestDelegate rd,
                                          ILogger<ServiceAuthorizeMiddleware> logger,
                                          IOptions<IdentitySettings> identitySettings)
        {
            _logger = logger;
            _identitySettings = identitySettings;
            _next = rd;
        }

        /// <summary>
        /// Eğer ServiceName ve ClientKey geçerli ise o kullanıcıyı 1111-1111-1111-1111 kullanıcısı ile login olmuş kabul eder
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            string serviceName = httpContext.Request.Headers["ServiceName"];
            string clientKey = httpContext.Request.Headers["ClientKey"];

            if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(clientKey))
            {
                if (!Config.GetApis().Any(x => x.Name.Equals(serviceName)) || !clientKey.Equals(_identitySettings.Value.ClientKey))
                {
                    _logger.LogCritical($"CRITICAL: Tanınmayan bir yerden bizim servisimizmiş gibi login olmaya çalışıyor. serviceName:{serviceName} clientKey:{clientKey}");

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    return;
                }

                _logger.LogInformation($"{serviceName} isimli servisten istek alındı.");

                var identity = new ClaimsIdentity("cookies");

                identity.AddClaim(new Claim("sub", USER_ID));
                identity.AddClaim(new Claim("unique_name", USER_ID));
                identity.AddClaim(new Claim("IsService", "true"));// Bizim servisimiz olduğunu policy anlaması için bu değeri atadık

                httpContext.User.AddIdentity(identity);
            }

            await _next.Invoke(httpContext);
        }
    }
}