using System;
using System.Collections.Generic;
using OnionCore;

namespace ServiceInterface.IControllers
{
    /// <summary>
    /// Main controller interface.
    /// </summary>
    public interface IMainController : IController
    {
        /// <summary>
        /// A <c>ProductController</c>.
        /// </summary>
        public IProductController ProductController { get; set; }
        
        /// <summary>
        /// An <c>OrderController</c>.
        /// </summary>
        public IOrderController OrderController { get; set; }
        
        /// <summary>
        /// A <c>IUserController</c>.
        /// </summary>
        public IUserController UserController { get; set; }

        /// <summary>
        /// Delegates for guest commands.
        /// </summary>
        /// <returns>Dictionary of string - Delegate, where string is the terminal command
        /// and Delegate is the corresponding method.</returns>
        public IDictionary<string, Delegate> GetDelegates();
        
        /// <summary>
        /// Delegates for the specified <c>IUser</c>.
        /// </summary>
        /// <param name="user">The <c>IUser</c> who requested delegates.</param>
        /// <returns>Admin delegates if the <paramref name="user"/> is admin,
        /// user delegates if the <paramref name="user"/> is registered,
        /// or guest delegates in other cases (including null argument).</returns>
        public IDictionary<string, Delegate> GetDelegates(IUser user);
    }
}