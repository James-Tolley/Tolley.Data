using System;

namespace Tolley.Data
{
    /// <summary>
    /// A unit of work on a <see cref="IDataContext"/>
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Save all changes
        /// </summary>
        void SaveChanges();
    }
}
