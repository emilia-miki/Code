using System.Collections.Generic;

namespace RepositoryInterface
{
    /// <summary>
    /// Repository interface.
    /// </summary>
    /// <typeparam name="T">The type of objects in the repository.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Get item by <paramref name="id">ID</paramref>.
        /// </summary>
        /// <param name="id">Item ID.</param>
        /// <returns>The item with the specified <paramref name="id">ID</paramref>.</returns>
        public T GetById(int id);
        
        /// <summary>
        /// Get all items.
        /// </summary>
        /// <returns>A list of all items in the repository.</returns>
        public IEnumerable<T> GetAll();
        
        /// <summary>
        /// Add a new item to repository.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The added item.</returns>
        public T Add(T item);
        
        /// <summary>
        /// Remove an item by <paramref name="id">ID</paramref>.
        /// </summary>
        /// <param name="id">The item ID.</param>
        public void RemoveById(int id);
    }
}