using System.Collections.Generic;
using OnionCore;

namespace RepositoryInterface
{
    /// <summary>
    /// <c>Order</c> repository interface. Implements <c>IRepository</c>.
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        /// Gets orders by <paramref name="id">customer ID</paramref>.
        /// </summary>
        /// <param name="id">Customer ID.</param>
        /// <returns>The list of the customer's orders.</returns>
        public IEnumerable<Order> GetByCustomerId(int id);
    }
}