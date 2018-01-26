namespace Tolley.Data
{
    /// <summary>
    /// A Database Context
    /// </summary>
    public interface IDataContext
    {
        /// <summary>
        /// Begin a new unit of work on this context
        /// </summary>
        /// <returns></returns>
        IUnitOfWork BeginScope();
    }
}
