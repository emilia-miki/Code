using System;
using System.Collections.Generic;
using OnionCore;

namespace ServiceInterface.IControllers
{
    /// <summary>
    /// User controller interface.
    /// </summary>
    public interface IUserController : IController
    {
        /// <summary>
        /// Registers a new <c>RegisteredUser</c>.
        /// </summary>
        /// <param name="login">Login.</param>
        /// <param name="password">Password.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string Register(string login, string password);
        
        /// <summary>
        /// Login method.
        /// </summary>
        /// <param name="login">Login.</param>
        /// <param name="password">Password.</param>
        /// <returns>
        /// Dictionary of all commands and corresponding delegates
        /// that are available to the <c>IUser</c> who logged in.
        /// If login failed, guest delegates are returned.
        /// </returns>
        public IDictionary<string, Delegate> Login(string login, string password);
        
        /// <summary>
        /// Sign out.
        /// </summary>
        /// <returns>Guest delegates dictionary.</returns>
        public IDictionary<string, Delegate> SignOut();
        
        /// <summary>
        /// View <c>PersonalInformation</c> of all <c>RegisteredUser</c> entries.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <returns>List of (id, login, <c>PersonalInformation</c>) entries for all <c>RegisteredUser</c> entries
        /// or empty list if authorization failed.</returns>
        public IEnumerable<Tuple<int, string, PersonalInformation>> ViewUsersPersonalInformation(IUser argUser);
        
        /// <summary>
        /// Change <c>PersonalInformation</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="personalInformation">New <c>PersonalInformation</c>.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string ChangePersonalInformation(IUser argUser, PersonalInformation personalInformation);

        /// <summary>
        /// Change <c>IUser</c> login and password.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="newLogin">New login.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string ChangeCredentials(IUser argUser, string newLogin, string newPassword);

        /// <summary>
        /// Change <c>IUser</c> <c>PersonalInformation</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="userId"><c>IUser</c> ID.</param>
        /// <param name="personalInformation">New <c>PersonalInformation</c>.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string ChangeUserPersonalInformation(IUser argUser, int userId, 
            PersonalInformation personalInformation);
    }
}