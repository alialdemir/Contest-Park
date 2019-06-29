using ContestPark.Core.Dapper.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ContestPark.Core.Dapper.Abctract
{
    public class DatabaseConnection : Disposable, IDatabaseConnection
    {
        #region Constructor

        public DatabaseConnection(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        #endregion Constructor

        #region Properties

        private IDbConnection _connection;

        public IDbConnection Connection
        {
            get
            {
                if (_connection.State != ConnectionState.Open && _connection.State != ConnectionState.Connecting)
                    _connection.Open();

                return _connection;
            }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Close the connection if this is open
        /// </summary>
        protected override void DisposeCore()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                //   SqlConnection.ClearAllPools();
            }

            base.DisposeCore();
        }

        #endregion Overrides
    }
}
