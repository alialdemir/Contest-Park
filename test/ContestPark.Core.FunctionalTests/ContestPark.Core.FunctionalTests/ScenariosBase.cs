using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Xunit;

namespace ContestPark.Core.FunctionalTests
{
    [TestCaseOrderer("ContestPark.Core.FunctionalTests.TestCaseOrdering.PriorityOrderer", "ContestPark.Core.FunctionalTests")]
    public class ScenariosBase<TStartup> where TStartup : class
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(ScenariosBase<TStartup>))
              .Location;

            var hostBuilder = new WebHostBuilder()
                  .UseContentRoot(Path.GetDirectoryName(path))
                  .ConfigureAppConfiguration(cb =>
                  {
                      cb.AddJsonFile("appsettings.test.json", optional: false);
                      cb.AddEnvironmentVariables();
                  })
                .UseStartup<TStartup>();

            var testServer = new TestServer(hostBuilder);

            Seed(testServer.Host);

            Conf.Configuration = testServer.Host.Services.GetRequiredService<IConfiguration>();

            return testServer;
        }

        public virtual void Seed(IWebHost host)
        {
        }

        public string GetErrorMessage(HttpResponseMessage response)
        {
            var jsonData = response.Content.ReadAsStringAsync().Result;

            ValidationMessage result = JsonConvert.DeserializeObject<ValidationMessage>(jsonData);

            return result.ErrorMessage;
        }
    }

    public class Conf
    {
        public static IConfiguration Configuration { get; set; }
    }
}