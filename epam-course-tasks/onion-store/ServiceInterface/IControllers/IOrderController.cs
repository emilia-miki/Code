using System.Collections.Generic;
using OnionCore;

namespace ServiceInterface.IControllers
{
    /// <summary>
    /// Order controller interface.
    /// </summary>
    public interface IOrderController : IController
    {
        /// <summary>
        /// Create a new <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called this method.</param>
        /// <param name="productList">A list of <c>ProductAndAmount</c> entries.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string CreateOrder(IUser argUser, IEnumerable<ProductAndAmount> productList);
        
        /// <summary>
        /// Pay for an <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who pays.</param>
        /// <param name="orderId">ID of the <c>Order</c>.</param>
        /// <returns>
        /// String description of the result of method execution.
        /// </returns>
        public string PayOrder(IUser argUser, int orderId);
        
        /// <summary>
        /// Cancel an <c>Order</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who cancels an <c>Order</c>.</param>
        /// <param name="orderId">ID of the <c>Order</c> to cancel.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string CancelOrder(IUser argUser, int orderId);
        
        /// <summary>
        /// Review <c>Order</c> history.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called this method.</param>
        /// <returns>The list of all <c>Order</c> entries in chronological order
        /// or an empty list if authorization failed.</returns>
        public IEnumerable<Order> ReviewOrderHistory(IUser argUser);
        
        /// <summary>
        /// Set <c>OrderStatus</c> to Received.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="orderId">The <c>Order</c> ID.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string SetOrderStatusReceived(IUser argUser, int orderId);
        
        /// <summary>
        /// Change <c>OrderStatus</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="orderId">ID of the <c>Order</c>.</param>
        /// <param name="status">The new <c>StatusOrder</c>.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string ChangeOrderStatus(IUser argUser, int orderId, OrderStatus status);
    }
}