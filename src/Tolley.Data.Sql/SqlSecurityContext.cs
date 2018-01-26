namespace Tolley.Data.Sql
{
    /// <summary>
    /// Defines a the encryption requirements for a sql connection
    /// </summary>
    public class SqlSecurityContext
    {
        /// <summary>
        /// Sql Connections should be encrypted
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Symmetric key name
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Decryption certificate
        /// </summary>
        public string Certificate { get; set; }
    }
}
