namespace Tolley.Data.Sql
{
    /// <summary>
    /// Allow access to the current security context
    /// </summary>
    public interface ISqlSecurityContextAccessor
    {
        /// <summary>
        /// Returns the current security context
        /// </summary>
        /// <returns></returns>
        SqlSecurityContext SecurityContext();
    }
}
