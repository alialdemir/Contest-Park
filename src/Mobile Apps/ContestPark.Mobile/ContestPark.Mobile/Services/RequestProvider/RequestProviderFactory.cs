using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.RequestProvider
{
    public class RequestProviderFactory
    {
        #region Methods

        public RequestProvider CreateResilientHttpClient()
        {
            return new RequestProvider(origin => CreatePolicies());
        }

        private IEnumerable<AsyncPolicy> CreatePolicies()
        {
            var waitAndRetryPolicy = Policy.Handle<HttpRequestException>()
                // Policy 1: wait and retry policy (bypasses internet connectivity issues)
                .WaitAndRetryAsync(
                    // number of retries
                    6,
                    // exponential backofff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        //var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                        //    $"of {context.PolicyKey} " +
                        //    $"at {context.ExecutionKey}, " +
                        //    $"due to: {exception}.";
                    });

            // Policy 2: circuit breaker (not overload the server)
            var circuitBreakerPolicy = Policy.Handle<HttpRequestException>()

        .FallbackAsync(
             (cancellationToken) =>
             {
                 return Task.CompletedTask;
             });
            //.CircuitBreakerAsync(
            //    // number of exceptions before breaking circuit
            //    10,
            //    // time circuit opened before retry
            //    TimeSpan.FromMinutes(40),
            //    (exception, duration) =>
            //    {
            //        // on circuit opened
            //        //_logger.LogTrace("Circuit breaker opened");
            //    },
            //    () =>
            //    {
            //        // on circuit closed
            //        //_logger.LogTrace("Circuit breaker reset");
            //    })-;

            return new List<AsyncPolicy>
            {
                waitAndRetryPolicy,
                circuitBreakerPolicy
            };
        }

        #endregion Methods
    }
}