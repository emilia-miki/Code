using System;
using System.Collections.Generic;
using RepositoryInterface;
using OnionCore;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;
using ServiceInterface.Services;

namespace ServiceInterface.Controllers
{
    /// <summary>
    /// The main controller that creates other specific controllers
    /// and uses <c>IDelegateService</c> to return the delegates of guest or user commands.
    /// </summary>
    public class MainController : IMainController
    {
        /// <inheritdoc cref="IController.UserRepository"/>
        public IUserRepository UserRepository { get; set; }
        
        /// <inheritdoc cref="IController.ProductRepository"/>
        public IProductRepository ProductRepository { get; set; }

        /// <inheritdoc cref="IController.ProductAndAmountRepository"/>
        public IProductAndAmountRepository ProductAndAmountRepository { get; set; }

        /// <inheritdoc cref="IController.PersonalInformationRepository"/>
        public IPersonalInformationRepository PersonalInformationRepository { get; set; }

        /// <inheritdoc cref="IController.OrderRepository"/>
        public IOrderRepository OrderRepository { get; set; }
        
        /// <inheritdoc cref="IController.AuthorizationService"/>
        public IAuthorizationService AuthorizationService { get; set; }
        
        /// <inheritdoc cref="IController.DelegateService"/>
        public IDelegateService DelegateService { get; set; }

        /// <inheritdoc cref="IMainController.UserController"/>
        public IUserController UserController { get; set; }
        
        /// <inheritdoc cref="IMainController.OrderController"/>
        public IOrderController OrderController { get; set; }
        
        /// <inheritdoc cref="IMainController.ProductRepository"/>
        public IProductController ProductController { get; set; }

        /// <summary>
        /// MainController constructor.
        /// </summary>
        /// <param name="userRepository">User repository to use.</param>
        /// <param name="productRepository">Product repository to use.</param>
        /// <param name="orderRepository">Order repository to use.</param>
        public MainController(IUserRepository userRepository,
            IProductRepository productRepository, IOrderRepository orderRepository)
        {
            OrderRepository = orderRepository;
            UserRepository = userRepository;
            ProductRepository = productRepository;

            AuthorizationService = new AuthorizationService();

            UserController = new UserController();
            OrderController = new OrderController();
            ProductController = new ProductController();

            DelegateService = new DelegateService
            {
                UserController = UserController,
                ProductController = ProductController,
                OrderController = OrderController
            };

            foreach (var controller in new IController[] {UserController, OrderController, ProductController})
            {
                controller.UserRepository = UserRepository;
                controller.ProductRepository = ProductRepository;
                controller.OrderRepository = OrderRepository;
                controller.AuthorizationService = AuthorizationService;
                controller.DelegateService = DelegateService;
            }

            AuthorizationService.UserRepository = UserRepository;
            AuthorizationService.DelegateService = DelegateService;
        }

        /// <inheritdoc cref="IMainController.GetDelegates()"/>
        public IDictionary<string, Delegate> GetDelegates()
        {
            return GetDelegates(null);
        }
        
        /// <inheritdoc cref="IMainController.GetDelegates(OnionCore.IUser)"/>
        public IDictionary<string, Delegate> GetDelegates(IUser user)
        {
            return user == null ? DelegateService.GuestDelegates : UserController.Login(user.Login, user.Password);
        }
    }
}