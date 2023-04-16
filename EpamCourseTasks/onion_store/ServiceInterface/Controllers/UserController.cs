using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RepositoryInterface;
using OnionCore;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;

namespace ServiceInterface.Controllers
{
    /// <summary>
    /// <c>IUser</c> controller. Implements <c>IUserController</c>.
    /// </summary>
    public class UserController : IUserController
    {
        /// <inheritdoc cref="IController.ProductRepository"/>
        public IProductRepository ProductRepository { get; set; }

        /// <inheritdoc cref="IController.ProductAndAmountRepository"/>
        public IProductAndAmountRepository ProductAndAmountRepository { get; set; }

        /// <inheritdoc cref="IController.PersonalInformationRepository"/>
        public IPersonalInformationRepository PersonalInformationRepository { get; set; }

        /// <inheritdoc cref="IController.OrderRepository"/>
        public IOrderRepository OrderRepository { get; set; }
        
        /// <inheritdoc cref="IController.UserRepository"/>
        public IUserRepository UserRepository { get; set; }
        
        /// <inheritdoc cref="IController.DelegateService"/>
        public IDelegateService DelegateService { get; set; }
        
        /// <inheritdoc cref="IController.AuthorizationService"/>
        public IAuthorizationService AuthorizationService { get; set; }

        /// <summary>
        /// Registers a new <c>RegisteredUser</c>.
        /// </summary>
        /// <param name="login">Login.</param>
        /// <param name="password">Password.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "This login is already taken",
        /// "You have been registered successfully".
        /// </returns>
        [Command("register")]
        [RolePermissions(Role.Guest)]
        public string Register(string login, string password)
        {
            if (UserRepository.GetByLogin(login) != default)
            {
                return "This login is already taken";
            }

            var user = new RegisteredUser
            {
                Login = login,
                Password = password,
                PersonalInformation = PersonalInformationRepository.Add(new PersonalInformation())
            };

            UserRepository.Add(user);

            return "You have been registered successfully";
        }
        
        /// <inheritdoc cref="IUserController.Login" />
        [Command("login")]
        [RolePermissions(Role.Guest)]
        public IDictionary<string, Delegate> Login(string login, string password)
        {
            var user = UserRepository.GetByLogin(login);
            if (user == default || user.Password != password)
            {
                return DelegateService.GuestDelegates;
            }

            return user is Admin ? DelegateService.AdminDelegates : DelegateService.UserDelegates;
        }
        
        /// <inheritdoc cref="IUserController.SignOut" />
        [Command("signOut")]
        [RolePermissions(Role.RegisteredUser, Role.Admin)]
        public IDictionary<string, Delegate> SignOut()
        {
            return DelegateService.GuestDelegates;
        }
        
        /// <inheritdoc cref="IUserController.ViewUsersPersonalInformation" />
        [Command("viewUsersPersonalInformation")]
        [RolePermissions(Role.Admin)]
        public IEnumerable<Tuple<int, string, PersonalInformation>> ViewUsersPersonalInformation(IUser argUser)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            return AuthorizationService.Authorize(argUser, attribute!.Roles)
                ? UserRepository.GetAllRegisteredUsers().Select(u =>
                    new Tuple<int, string, PersonalInformation>(u.Id, u.Login,
                        PersonalInformationRepository.GetById(u.PersonalInformation.Id)))
                : new List<Tuple<int, string, PersonalInformation>>();
        }

        /// <summary>
        /// Change <c>PersonalInformation</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="personalInformation">New <c>PersonalInformation</c>.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "Profile information has been successfully changed".
        /// </returns>
        [Command("changePersonalInformation")]
        [RolePermissions(Role.RegisteredUser, Role.Admin)]
        public string ChangePersonalInformation(IUser argUser, PersonalInformation personalInformation)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var user = UserRepository.GetByLogin(argUser.Login);
            var userInfo = PersonalInformationRepository.GetById(user.PersonalInformation.Id);
            user.PersonalInformation = userInfo;
            userInfo.Name = personalInformation.Name;
            userInfo.Surname = personalInformation.Surname;
            userInfo.Email = personalInformation.Email;
            userInfo.PhoneNumber = personalInformation.PhoneNumber;
            return "Profile information has been successfully changed";
        }

        /// <inheritdoc cref="IUserController.ChangeCredentials"/>
        [Command("changeCredentials")]
        [RolePermissions(Role.RegisteredUser, Role.Admin)]
        public string ChangeCredentials(IUser argUser, string newLogin, string newPassword)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var user = UserRepository.GetByLogin(argUser.Login);
            user.Login = newLogin;
            user.Password = newPassword;
            return "Credentials have been successfully changed";
        }

        /// <summary>
        /// Change <c>IUser</c> <c>PersonalInformation</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="userId"><c>IUser</c> ID.</param>
        /// <param name="personalInformation">New <c>PersonalInformation</c>.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "The specified user doesn't exist",
        /// "Personal information is null"
        /// "User personal information has been changed successfully".
        /// </returns>
        [Command("changeUserPersonalInformation")]
        [RolePermissions(Role.Admin)]
        public string ChangeUserPersonalInformation(IUser argUser, int userId,
            PersonalInformation personalInformation)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var user = UserRepository.GetById(userId);
            if (user == default)
            {
                return "The specified user doesn't exist";
            }
            
            if (personalInformation == null)
            {
                return "Personal information is null";
            }

            var userInfo = PersonalInformationRepository.GetById(user.PersonalInformation.Id);
            user.PersonalInformation = userInfo;
            userInfo.Name = personalInformation.Name;
            userInfo.Surname = personalInformation.Surname;
            userInfo.Email = personalInformation.Email;
            userInfo.PhoneNumber = personalInformation.PhoneNumber;
            return "User personal information has been changed successfully";
        }
    }
}