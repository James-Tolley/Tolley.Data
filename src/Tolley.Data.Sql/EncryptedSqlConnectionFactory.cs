using System.Data;
using System.Threading.Tasks;

namespace Tolley.Data.Sql
{
    /// <summary>
    /// Creates encrypted SQL Connections
    /// </summary>
    public class EncryptedSqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;
        private readonly ISqlSecurityContextAccessor _securityContextAccessor;

        /// <summary>
        /// Create a new <see cref="SqlConnectionFactory"/>
        /// </summary>
        /// <param name="connectionString">ADO Connection string</param>
        /// <param name="securityContextAccessor"></param>
        public EncryptedSqlConnectionFactory(string connectionString, ISqlSecurityContextAccessor securityContextAccessor)
        {
            _connectionString = connectionString;
            _securityContextAccessor = securityContextAccessor;
        }

        /// <inheritdoc />
        public IDbConnection GetConnection()
        {
            var connection = new EncryptedDbConnection(_connectionString, _securityContextAccessor.SecurityContext());
            connection.Open();

            return connection;
        }

        /// <inheritdoc />
        public async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = new EncryptedDbConnection(_connectionString, _securityContextAccessor.SecurityContext());
            await connection.OpenAsync();

            return connection;
        }
    }
}
