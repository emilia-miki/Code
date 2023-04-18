using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceInterface.Controllers;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;

namespace ServiceInterface.Services
{
    /// <summary>
    /// Delegate service; implements IDelegateService.
    /// </summary>
    public class DelegateService : IDelegateService
    {
        private IProductController _productController;
        private IUserController _userController;
        private IOrderController _orderController;

        /// <inheritdoc cref="IMainController.ProductController"/>
        public IProductController ProductController
        {
            get => _productController;
            set
            {
                AssembleDelegatesFor(value);
                _productController = value;
            }
        }

        /// <inheritdoc cref="IMainController.UserController"/>
        public IUserController UserController
        {
            get => _userController;
            set
            {
                AssembleDelegatesFor(value);
                _userController = value;
            }
        }

        /// <inheritdoc cref="IMainController.OrderController"/>
        public IOrderController OrderController
        {
            get => _orderController;
            set
            {
                AssembleDelegatesFor(value);
                _orderController = value;
            }
        }
        
        /// <inheritdoc cref="IDelegateService.GuestDelegates"/>
        public IDictionary<string, Delegate> GuestDelegates { get; }
        
        /// <inheritdoc cref="IDelegateService.UserDelegates"/>
        public IDictionary<string, Delegate> UserDelegates { get; }
        
        /// <inheritdoc cref="IDelegateService.AdminDelegates"/>
        public IDictionary<string, Delegate> AdminDelegates { get; }

        /// <summary>
        /// DelegateService constructor. Initializes delegates dictionaries.
        /// </summary>
        public DelegateService()
        {
            GuestDelegates = new Dictionary<string, Delegate>();
            UserDelegates = new Dictionary<string, Delegate>();
            AdminDelegates = new Dictionary<string, Delegate>();
        }

        /// <summary>
        /// Assembles delegates for the specified <c>IController</c>.
        /// </summary>
        /// <param name="controller"><c>IController</c>.</param>
        private void AssembleDelegatesFor(IController controller)
        {
            var controllerType = controller.GetType();
            foreach (var methodInfo in controllerType.GetMethods())
            {
                var commandAttribute = (CommandAttribute)
                    methodInfo.GetCustomAttribute(typeof(CommandAttribute));
                var rolesAttribute = (RolePermissionsAttribute) 
                    methodInfo.GetCustomAttribute(typeof(RolePermissionsAttribute));
                if (commandAttribute == null || rolesAttribute == null)
                {
                    continue;
                }
                    
                var parameters = methodInfo.GetParameters();
                var types = parameters.Select(p => p.ParameterType).ToList();
                types.Add(methodInfo.ReturnType);
                var delegateType = Expression.GetFuncType(types.ToArray());

                var command = commandAttribute!.Command;
                var del = methodInfo.CreateDelegate(delegateType, controller);
                    
                if (rolesAttribute!.Roles.Any(r => r == Role.Guest))
                {
                    GuestDelegates.Add(command, del);
                }
                    
                if (rolesAttribute!.Roles.Any(r => r == Role.RegisteredUser))
                {
                    UserDelegates.Add(command, del);
                }

                if (rolesAttribute!.Roles.Any(r => r == Role.Admin))
                {
                    AdminDelegates.Add(command, del);
                }
            }
        }
    }
}