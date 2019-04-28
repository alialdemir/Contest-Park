using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ContestPark.Core.DataSeed
{
    public abstract class ContextSeedBase
    {
        public string ConnectionString { get; set; }

        public ILogger Logger { get; set; }

        public string SeedName { get; set; }

        public AsyncRetryPolicy CreatePolicy(int retries = 3)
        {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        Logger.LogTrace($"[{SeedName}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }

        public async Task InsertDataAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity, new()
        {
            Logger.LogInformation($"The entities are being added. {typeof(TEntity).Name}");

            DapperRepositoryBase<TEntity> repository = new DapperRepositoryBase<TEntity>(ConnectionString);

            if (repository.GetCount() == 0)
            {
                await repository.InsertAsync(entities);
            }

            Logger.LogInformation($"The entities have been added.. {typeof(TEntity).Name}");
        }

        public abstract Task SeedAsync(ISettingsBase settings, ILogger logger);
    }
}