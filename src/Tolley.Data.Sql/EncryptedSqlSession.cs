using System;
using System.Data;

namespace Tolley.Data.Sql
{
    /// <summary>
    /// Represents a sql session where data encryption/decryption is taking place.
    /// Key is opened when the session starts and closed when the session ends
    /// </summary>
    public class EncryptedSqlSession : IDisposable
    {
        private bool _isClosed = true;
        private readonly SqlSecurityContext _context;
        private readonly IDbConnection _connection;

        /// <summary>
        /// Create a new session
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="context"></param>
        public EncryptedSqlSession(IDbConnection connection, SqlSecurityContext context)
        {
            _connection = connection;
            _context = context;

            Open();
        }

        /// <summary>
        /// Opens the encryption key
        /// </summary>
        public void Open()
        {
            if (!_context.IsEncrypted || _connection.State != ConnectionState.Open) return;

            IDbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = $"OPEN SYMMETRIC KEY {_context.KeyName} DECRYPTION BY CERTIFICATE {_context.Certificate};";
            cmd.ExecuteNonQuery();
            _isClosed = false;
        }

        /// <summary>
        /// Closes the encryption key
        /// </summary>
        public void Close()
        {
            if (_isClosed) return;

            if (_context.IsEncrypted && _connection.State == ConnectionState.Open)
            {
                IDbCommand cmd = _connection.CreateCommand();
                cmd.CommandText = $"CLOSE SYMMETRIC KEY {_context.KeyName};";
                cmd.ExecuteNonQuery();
            }

            _isClosed = true;
        }

        /// <summary>
        /// Automatically closes the key on disposal
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
