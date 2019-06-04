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

        public async Task InsertDataAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IDocument, new()
        {
            Logger.LogInformation($"The entities are being added. {typeof(TEntity).Name}");

            IDocumentDbRepository<TEntity> dbRepository = (IDocumentDbRepository<TEntity>)Service.GetRequiredService(typeof(IDocumentDbRepository<TEntity>));

            await dbRepository.Init();// database ve collection oluşturuldu

            if (await dbRepository.CountAsync() == 0)
            {
                await dbRepository.InsertAsync(entities);
            }

            Logger.LogInformation($"The entities have been added.. {typeof(TEntity).Name}");
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