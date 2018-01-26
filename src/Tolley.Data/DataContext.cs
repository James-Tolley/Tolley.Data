using System;
using System.Data;

namespace Tolley.Data
{
    /// <summary>
    /// A data context on an <see cref="IDbConnection"/>
    /// </summary>
    public class DataContext : IDataContext, IDisposable
    {
        private int _transactionCount;
        private bool _disposed;

        /// <summary>
        /// Private unit of work handles transactions on the data context
        /// </summary>
        private class UnitOfWork : IUnitOfWork
        {
            private readonly DataContext _context;
            private bool _isCommitted = false;
            private bool _disposed = false;

            /// <summary>
            /// Create a new unit of work on a <see cref="DataContext"/>
            /// </summary>
            /// <param name="context"></param>
            public UnitOfWork(DataContext context)
            {
                _context = context;
                _context.BeginTransaction();
            }

            /// <inheritdoc />
            public void SaveChanges()
            {
                if (_isCommitted) throw new InvalidOperationException("Unit of Work changes already saved");

                _context.CommitTransaction();
                _isCommitted = true;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing && !_isCommitted)
                {
                    _context.RollbackTransaction();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Create a new data context
        /// </summary>
        /// <param name="factory">Provides connections</param>
        public DataContext(IConnectionFactory factory)
        {
            Connection = factory.GetConnection();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Transaction?.Rollback();
                CleanupTransaction();

                Connection?.Close();
                Connection?.Dispose();
                Connection = null;
            }

            _disposed = true;
        }

        private void CleanupTransaction()
        {
            Transaction?.Dispose();
            Transaction = null;
        }

        /// <summary>
        /// Begin a transaction on the DataContext connection
        /// </summary>
        /// <returns></returns>
        private void BeginTransaction()
        {
            _transactionCount++;
            Transaction = Transaction ?? Connection.BeginTransaction();
        }

        /// <summary>
        /// Commit current transaction
        /// </summary>
        private void CommitTransaction()
        {
            --_transactionCount;
            if (_transactionCount != 0)
            {
                return;
            }

            Transaction?.Commit();
            CleanupTransaction();
        }

        /// <summary>
        /// Rollback all transactions
        /// </summary>
        private void RollbackTransaction()
        {
            _transactionCount = 0;

            Transaction?.Rollback();
            CleanupTransaction();
        }

        /// <inheritdoc />
        public IUnitOfWork BeginScope()
        {
            return new UnitOfWork(this);
        }

        /// <summary>
        /// Connection to database
        /// </summary>
        public IDbConnection Connection { get; private set; }

        /// <summary>
        /// Current transaction
        /// </summary>
        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Identify this instance
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();
    }
}
