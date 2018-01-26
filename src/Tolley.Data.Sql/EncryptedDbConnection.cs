using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Tolley.Data.Sql.Internal;

namespace Tolley.Data.Sql
{
    /// <summary>
    /// A sql database connection that uses an encryption key
    /// </summary>

    public class EncryptedDbConnection : DbConnection
    {
        private readonly DbConnection _sqlConnection;
        private readonly SqlSecurityContext _securityContext;
        private EncryptedSqlSession _encryptedSession;

        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="securityContext">Security context</param>
        public EncryptedDbConnection(string connectionString, SqlSecurityContext securityContext)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _securityContext = securityContext;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Close();

            if (disposing)
            {
                _encryptedSession.Dispose();
                _sqlConnection.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _sqlConnection.BeginTransaction(isolationLevel);
        }

        /// <inheritdoc />
        public override void ChangeDatabase(string databaseName)
        {
            _sqlConnection.ChangeDatabase(databaseName);
        }

        /// <inheritdoc />
        public override void Close()
        {
            _encryptedSession.Close();
            _sqlConnection.Close();
        }

        /// <inheritdoc />
        public override void Open()
        {
            _sqlConnection.Open();
            _encryptedSession = _sqlConnection.OpenKey(_securityContext);
        }

        /// <inheritdoc />
        public override string ConnectionString
        {
            get => _sqlConnection.ConnectionString;
            set => _sqlConnection.ConnectionString = value;
        }

        /// <inheritdoc />
        public override string Database => _sqlConnection.Database;

        /// <inheritdoc />
        public override ConnectionState State => _sqlConnection.State;

        /// <inheritdoc />
        public override string DataSource => _sqlConnection.DataSource;

        /// <inheritdoc />
        public override string ServerVersion => _sqlConnection.ServerVersion;

        /// <inheritdoc />
        protected override DbCommand CreateDbCommand()
        {
            return _sqlConnection.CreateCommand();
        }
    }
}
