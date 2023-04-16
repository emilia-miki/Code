using System.Collections.Generic;
using OnionCore;

namespace ServiceInterface.IControllers
{
    /// <summary>
    /// Product controller interface
    /// </summary>
    public interface IProductController : IController
    {
        /// <summary>
        /// Add new <c>Product</c>
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method</param>
        /// <param name="name">Name</param>
        /// <param name="category">Category</param>
        /// <param name="description">Description</param>
        /// <param name="cost">Cost</param>
        /// <returns>
        /// String description of the result of method execution
        /// </returns>
        public string AddNewProduct(IUser argUser, string name, string category, string description, decimal cost);

        /// <summary>
        /// Change <c>Product</c> information.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="productId">ID of the <c>Product</c>.</param>
        /// <param name="name">New name.</param>
        /// <param name="category">New category.</param>
        /// <param name="description">New description.</param>
        /// <param name="cost">New cost.</param>
        /// <returns>String description of the result of method execution.</returns>
        public string ChangeProductInformation(IUser argUser, int productId, string name, string category,
            string description, decimal cost);

        /// <summary>
        /// View all <c>Product</c> entries.
        /// </summary>
        /// <returns>The list of all <c>Product</c> entries.</returns>
        public IEnumerable<Product> ViewProducts();
        
        /// <summary>
        /// Find a <c>Product</c> by name.
        /// </summary>
        /// <param name="name"><c>Product</c> name.</param>
        /// <returns>If the <c>Product</c> has been found, it is returned; otherwise null is returned.</returns>
        public Product SearchProductByName(string name);
    }
}