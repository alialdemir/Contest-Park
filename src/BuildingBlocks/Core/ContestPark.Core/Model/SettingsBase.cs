using ContestPark.Core.Interfaces;
using System;

namespace ContestPark.Core.Model
{
    public class SettingsBase : ISettingsBase
    {
        private string _connectionString = string.Empty;

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                    _connectionString = GetEnvironment("ConnectionString");

                return _connectionString;
            }
        }

        private string GetEnvironment(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
    }
}