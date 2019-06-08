using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Core.CosmosDb.Infrastructure
{
    public abstract class ContextSeedBase<TContextSeed>
    {
        public ILogger<TContextSeed> Logger { get; set; }

        public IServiceProvider Service { get; set; }

        public async Task<bool> InsertDataAsync<TDocument>(IEnumerable<TDocument> entities) where TDocument : class, IDocument, new()
        {
            Logger.LogInformation($"The entities are being added. {typeof(TDocument).Name}");

            IDocumentDbRepository<TDocument> dbRepository = (IDocumentDbRepository<TDocument>)Service.GetRequiredService(typeof(IDocumentDbRepository<TDocument>));

            //    await dbRepository.Init();// database ve collection oluşturuldu

            if (await dbRepository.CountAsync() == 0)
            {
                return (await dbRepository.AddRangeAsync(entities));
            }

            Logger.LogInformation($"The entities have been added.. {typeof(TDocument).Name}");

            return false;
        }

        public AsyncRetryPolicy CreatePolicy(int retries = 3)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        Logger.LogTrace($"[{typeof(TContextSeed).Name}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}