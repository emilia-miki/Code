using System;
using System.Collections.Generic;
using RepositoryInterface;
using System.Linq;
using System.Reflection;
using OnionCore;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;

namespace ServiceInterface.Controllers
{
    /// <summary>
    /// <c>Order</c> controller. Implements <c>IOrderController</c>
    /// </summary>
    public class OrderController : IOrderController
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
        /// Create a new <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called this method.</param>
        /// <param name="productList">A list of <c>ProductAndAmount</c> entries.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This product does not exist",
        /// "Order created successfully".
        /// </returns>
        [Command("createOrder")]
        [RolePermissions(Role.RegisteredUser)]
        public string CreateOrder(IUser argUser, IEnumerable<ProductAndAmount> productList)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var user = UserRepository.GetByLogin(argUser.Login);

            var productAndAmounts = productList.ToList();
            if (productAndAmounts.Any(entry => 
                    ProductRepository.GetById(entry.Product.Id) == default))
            {
                return "This product does not exist";
            }

            var order = new Order
            {
                Customer = user,
                DateTime = DateTime.Now,
                ProductList = productAndAmounts.Select(pa => 
                    ProductAndAmountRepository.Add(new ProductAndAmount
                    {
                        Product = ProductRepository.GetById(pa.Product.Id),
                        Amount = pa.Amount
                    })),
                Status = OrderStatus.New
            };

            OrderRepository.Add(order);

            return "Order created successfully";
        }

        /// <summary>
        /// Pay for an <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who pays.</param>
        /// <param name="orderId">ID of the <c>Order</c>.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This order doesn't exist",
        /// "Payment received".
        /// </returns>
        [Command("payOrder")]
        [RolePermissions(Role.RegisteredUser)]
        public string PayOrder(IUser argUser, int orderId)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var order = OrderRepository.GetById(orderId);
            if (order == default)
            {
                return "This order doesn't exist";
            }

            order.Status = OrderStatus.PaymentReceived;
            return "Payment received";
        }

        /// <summary>
        /// Cancel an <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who cancels an <c>Order</c>.</param>
        /// <param name="orderId">ID of the <c>Order</c> to cancel.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This order doesn't exist",
        /// "You can only cancel your own orders",
        /// "You can't cancel an order that has already been received",
        /// "The order has been successfully canceled".
        /// </returns>
        [Command("cancelOrder")]
        [RolePermissions(Role.RegisteredUser)]
        public string CancelOrder(IUser argUser, int orderId)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var order = OrderRepository.GetById(orderId);
            if (order == default)
            {
                return "This order doesn't exist";
            }

            if (argUser.Id != order.Customer.Id)
            {
                return "You can only cancel your own orders";
            }

            if (order.Status == OrderStatus.Received || order.Status == OrderStatus.Completed)
            {
                return "You can't cancel an order that has already been received";
            }

            order.Status = OrderStatus.CanceledByUser;
            return "The order has been successfully canceled";
        }

        /// <inheritdoc cref="IOrderController.ReviewOrderHistory" />
        [Command("reviewOrderHistory")]
        [RolePermissions(Role.RegisteredUser)]
        public IEnumerable<Order> ReviewOrderHistory(IUser argUser)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return new List<Order>();
            }

            var user = UserRepository.GetByLogin(argUser.Login);
            return OrderRepository.GetByCustomerId(user.Id).OrderByDescending(o => o.DateTime);
        }

        /// <summary>
        /// Set <c>OrderStatus</c> to Received.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="orderId">The <c>Order</c> ID.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This order doesn't exist",
        /// "Order status successfully set to Received".
        /// </returns>
        [Command("setOrderStatusReceived")]
        [RolePermissions(Role.RegisteredUser)]
        public string SetOrderStatusReceived(IUser argUser, int orderId)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var order = OrderRepository.GetById(orderId);
            if (order == default)
            {
                return "This order doesn't exist";
            }

            order.Status = OrderStatus.Received;

            return "Order status successfully set to Received";
        }

        /// <summary>
        /// Check if the specified <c>OrderStatus</c> change by the administrator is valid.
        /// </summary>
        /// <param name="original">The original <c>OrderStatus</c>.</param>
        /// <param name="status">The new <c>OrderStatus</c>.</param>
        /// <returns>True if the change is valid, otherwise false.</returns>
        public static bool IsOrderStatusChangePermitted(OrderStatus original, OrderStatus status)
        {
            return original == status 
                   || original == OrderStatus.New 
                   && (status == OrderStatus.Sent || status == OrderStatus.CanceledByTheAdministrator)
                   || original == OrderStatus.Sent && status == OrderStatus.CanceledByTheAdministrator
                   || original == OrderStatus.PaymentReceived && status == OrderStatus.CanceledByTheAdministrator
                   || original == OrderStatus.Received && status == OrderStatus.Completed;
        }

        /// <summary>
        /// Change <c>OrderStatus</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="orderId">ID of the <c>Order</c>.</param>
        /// <param name="status">The new <c>OrderStatus</c>.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This order does not exist",
        /// "This status change is illegal",
        /// "The order status has been changed".
        /// </returns>
        [Command("changeOrderStatus")]
        [RolePermissions(Role.Admin)]
        public string ChangeOrderStatus(IUser argUser, int orderId, OrderStatus status)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var order = OrderRepository.GetById(orderId);
            if (order == default)
            {
                return "This order does not exist";
            }

            if (!IsOrderStatusChangePermitted(order.Status, status))
            {
                return "This status change is illegal";
            }

            order.Status = status;
            return "The order status has been changed";
        }
    }
}