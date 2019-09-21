using ContestPark.Core.Dapper.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace ContestPark.Core.Dapper.Abctract
{
    public class DatabaseConnection : Disposable, IDatabaseConnection
    {
        #region Constructor

        public DatabaseConnection(IConfiguration configuration)
        {
            CreateConnection(configuration["ConnectionString"]);
        }

        public DatabaseConnection(string connectionString)
        {
            CreateConnection(connectionString);
        }

        private void CreateConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        #endregion Constructor

        #region Properties

        private MySqlConnection _connection;

        public IDbConnection Connection
        {
            get
            {
                try
                {
                    if (_connection.State != ConnectionState.Open && _connection.State != ConnectionState.Connecting)
                        _connection.Open();

                    return _connection;
                }
                catch (System.Exception ex)
                {
                    return _connection;
                }
            }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Close the connection if this is open
        /// </summary>
        public override void DisposeCore()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }

            base.DisposeCore();
        }

        #endregion Overrides
    }
}
