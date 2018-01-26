using System.Data;
using System.Threading.Tasks;

namespace Tolley.Data
{
    /// <summary>
    /// Creates database connections
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Opens a database connection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetConnection();

        /// <summary>
        /// Opens a database connection asynchronously
        /// </summary>
        /// <returns></returns>
        Task<IDbConnection> GetConnectionAsync();
    }
}
