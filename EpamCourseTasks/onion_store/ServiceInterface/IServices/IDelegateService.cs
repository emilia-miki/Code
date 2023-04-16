using System;
using System.Collections.Generic;
using ServiceInterface.IControllers;

namespace ServiceInterface.IServices
{
    /// <summary>
    /// Delegate service interface.
    /// </summary>
    public interface IDelegateService
    {
        /// <inheritdoc cref="IMainController.ProductController"/>
        public IProductController ProductController { get; set; }
        
        /// <inheritdoc cref="IMainController.UserController"/>
        public IUserController UserController { get; set; }
        
        /// <inheritdoc cref="IMainController.OrderController"/>
        public IOrderController OrderController { get; set; }

        /// <summary>
        /// A dictionary of commands and corresponding delegates available to guests.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, Delegate> GuestDelegates { get; }
        
        /// <summary>
        /// A dictionary of commands and corresponding delegates available to registered users.
        /// </summary>
        public IDictionary<string, Delegate> UserDelegates { get; }
        
        /// <summary>
        /// A dictionary of commands and corresponding delegates available to administrators.
        /// </summary>
        public IDictionary<string, Delegate> AdminDelegates { get; }
    }
}