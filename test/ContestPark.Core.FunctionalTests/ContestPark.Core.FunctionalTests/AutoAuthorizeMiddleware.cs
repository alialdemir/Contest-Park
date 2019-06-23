﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContestPark.Core.FunctionalTests
{
    public class AutoAuthorizeMiddleware
    {
        public const string IDENTITY_ID = "1111-1111-1111-1111";

        private readonly RequestDelegate _next;

        public AutoAuthorizeMiddleware(RequestDelegate rd)
        {
            _next = rd;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var identity = new ClaimsIdentity("cookies");

            identity.AddClaim(new Claim("sub", IDENTITY_ID));
            identity.AddClaim(new Claim("unique_name", IDENTITY_ID));
            identity.AddClaim(new Claim("IsService", "true"));// Bizim servisimiz olduğunu policy anlaması için bu değeri atadık

            httpContext.User.AddIdentity(identity);

            await _next.Invoke(httpContext);
        }
    }
}