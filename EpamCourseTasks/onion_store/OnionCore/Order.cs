using System;
using System.Collections.Generic;

namespace OnionCore
{
    /// <summary>
    /// Order class.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Order ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Order creation date.
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// List of products and corresponding amounts.
        /// </summary>
        public IEnumerable<ProductAndAmount> ProductList { get; set; }
        
        /// <summary>
        /// Order status.
        /// </summary>
        public OrderStatus Status { get; set; }
        
        /// <summary>
        /// The <c>IUser</c> who created the order.
        /// </summary>
        public IUser Customer { get; set; }
    }
}