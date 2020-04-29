using Amazon.CloudWatchLogs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using System;

namespace ContestPark.Core.Provider
{
    public class CustomLogStreamProvider : ILogStreamNameProvider
    {
        public CustomLogStreamProvider(string prefix)
        {
            this.prefix = prefix;
        }

        private readonly string prefix;

        public string GetLogStreamName()
        {
            return this.prefix;
        }
    }

    public static class LoggerConfigurationExtension
    {
        public static LoggerConfiguration AddAmazonCloudWatch(this LoggerConfiguration loggerConfiguration, IConfiguration configuration, string appName)
        {
            //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Production)// Sadece prod ortamında clouldwatch'a yazıyoruz
            //{
            var cloudWatchLogsClient = new AmazonCloudWatchLogsClient(configuration["AwsAccessKeyId"], configuration["AwsSecretAccessKey"], Amazon.RegionEndpoint.EUCentral1);
            loggerConfiguration.WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
            {
                LogGroupName = configuration["AwsLogGroupName"],
                LogStreamNameProvider = new CustomLogStreamProvider(appName),
                MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
                TextFormatter = new Serilog.Formatting.Json.JsonFormatter(),
            }, cloudWatchLogsClient);
            //}
            //else
            //{
            //    // loggerSinkConfiguration.Console();
            //}

            return loggerConfiguration;
        }
    }
}
