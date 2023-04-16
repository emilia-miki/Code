using OnionCore;

namespace RepositoryInterface
{
    /// <summary>
    /// <c>Product</c> repository interface. Implements <c>IRepository</c>.
    /// </summary>
    public interface IProductRepository : IRepository<Product>
    { 
        /// <summary>
        /// Get <c>Product</c> by name.
        /// </summary>
        /// <param name="name"><c>Product</c> name.</param>
        /// <returns>The <c>Product</c> with the specified name
        /// or null if a <c>Product</c> with this name doesn't exist.</returns>
        public Product GetByName(string name); 
    }
}
