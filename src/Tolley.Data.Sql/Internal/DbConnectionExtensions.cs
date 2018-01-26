using System.Data;

namespace Tolley.Data.Sql.Internal
{
    /// <summary>
    /// Adds extensiion method for DbConnection to open encryption key
    /// </summary>
    static class DbConnectionExtensions
    {
        /// <summary>
        /// Open an encryption key based on the current security context
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static EncryptedSqlSession OpenKey(this IDbConnection connection, SqlSecurityContext context)
        {
            return new EncryptedSqlSession(connection, context);
        }
    }
}
