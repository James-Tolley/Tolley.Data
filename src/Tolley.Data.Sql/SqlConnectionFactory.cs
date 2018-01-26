using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tolley.Data.Sql
{
    /// <summary>
    /// Creates SQL database connections
    /// </summary>
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Create a new <see cref="SqlConnectionFactory"/>
        /// </summary>
        /// <param name="connectionString">ADO Connection string</param>
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }

        /// <inheritdoc />
        public async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            return connection;
        }
    }
}
