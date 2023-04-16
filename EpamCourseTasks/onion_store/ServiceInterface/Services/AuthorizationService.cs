using System.Collections.Generic;
using System.Linq;
using RepositoryInterface;
using OnionCore;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;

namespace ServiceInterface.Services
{
    /// <summary>
    /// Authorization service.
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        /// <inheritdoc cref="IController.UserRepository"/>
        public IUserRepository UserRepository { get; set; }
        
        /// <inheritdoc cref="IController.DelegateService"/>
        public IDelegateService DelegateService { get; set; }

        /// <inheritdoc cref="IAuthorizationService.Authorize"/>
        public bool Authorize(IUser argUser, IEnumerable<Role> roles)
        {
            if (argUser == null)
            {
                return false;
            }

            var user = UserRepository.GetByLogin(argUser.Login);
            if (user == default || user.Password != argUser.Password)
            {
                return false;
            }

            if (user is Admin)
            {
                return roles.Any(r => r == Role.Admin);
            }
            
            if (user is RegisteredUser)
            {
                return roles.Any(r => r == Role.RegisteredUser);
            }

            return false;
        }
    }
}