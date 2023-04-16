using System.Collections.Generic;
using RepositoryInterface;
using OnionCore;
using ServiceInterface.IControllers;

namespace ServiceInterface.IServices
{
    /// <summary>
    /// Authorization service interface.
    /// </summary>
    public interface IAuthorizationService
    {
        /// <inheritdoc cref="IController.UserRepository"/>
        public IUserRepository UserRepository { get; set; }
        
        /// <inheritdoc cref="IController.ProductRepository"/>
        public IDelegateService DelegateService { get; set; }

        /// <summary>
        /// Authorize method.
        /// </summary>
        /// <param name="argUser">The user to authorize.</param>
        /// <param name="roles">The permitted roles.</param>
        /// <returns>True if the <paramref name="argUser"/> login and password are valid
        /// and they belong to one of the permitted roles; otherwise false.</returns>
        public bool Authorize(IUser argUser, IEnumerable<Role> roles);
    }
}