using System.Collections.Generic;
using OnionCore;

namespace RepositoryInterface
{
    /// <summary>
    /// <c>IUser</c> repository interface. Implements <c>IRepository</c>.
    /// </summary>
    public interface IUserRepository : IRepository<IUser>
    {
        /// <summary>
        /// Get a <c>IUser</c> by <paramref name="login"/>.
        /// </summary>
        /// <param name="login"><c>IUser</c> login</param>
        /// <returns>The <c>IUser</c> with the specified <paramref name="login"/>
        /// or null if such a <c>IUser</c> was not found.</returns>
        public IUser GetByLogin(string login);
        
        /// <summary>
        /// Get all <c>RegisteredUser</c> entries.
        /// </summary>
        /// <returns>The list of all <c>RegisteredUser</c> entries.</returns>
        public IEnumerable<RegisteredUser> GetAllRegisteredUsers();
    }
}