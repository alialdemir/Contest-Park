using Microsoft.Extensions.Configuration;
using System;

namespace ContestPark.Core.OrleansClient
{
    public class ClientBuilderContext
    {
        internal string ConnectionString
        {
            get
            {
                if (Configuration == null)
                    throw new ArgumentNullException(nameof(ConnectionString));

                string connectionString = Configuration["ConnectionString"];
                if (connectionString == null)
                    throw new ArgumentNullException(nameof(connectionString));

                return connectionString;
            }
        }

        internal string SiloHostIp
        {
            get
            {
                if (Configuration == null)
                    throw new ArgumentNullException(nameof(SiloHostIp));

                string siloHostIp = Configuration["SiloHostIp"];
                if (siloHostIp == null)
                    throw new ArgumentNullException(nameof(siloHostIp));

                return siloHostIp;
            }
        }

        internal int SiloHostPort
        {
            get
            {
                if (Configuration == null)
                    throw new ArgumentNullException(nameof(SiloHostPort));

                int siloHostPort = Configuration.GetValue<int>("SiloHostPort");
                if (siloHostPort <= 0)
                    throw new ArgumentNullException(nameof(siloHostPort));

                return siloHostPort;
            }
        }

        public IConfiguration Configuration { get; set; }
    }
}