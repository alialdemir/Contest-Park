using ContestPark.Core.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ContestPark.Core.Dapper
{
    public class DatabaseConnection : Disposable
    {
        #region Constructor

        public DatabaseConnection(ISettingsBase settingsBase)
        {
            if (settingsBase == null)
                throw new ArgumentNullException(nameof(settingsBase));
            string d = settingsBase.ConnectionString;

            _connection = new SqlConnection(settingsBase.ConnectionString);
        }

        public DatabaseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connection = new SqlConnection(connectionString);
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
    }
}