using System.Linq;
using MockLibrary;
using OnionCore;

namespace TestProject
{
    /// <summary>
    /// A helper class for unit testing.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Gets a <c>RegisteredUser</c>.
        /// </summary>
        /// <returns>A <c>RegisteredUser</c> from <c>MockData</c>.</returns>
        public static RegisteredUser GetRegisteredUser()
        {
            return MockIUserRepository.GetMock().Object.GetAllRegisteredUsers().First();
        }

        /// <summary>
        /// Gets an admin.
        /// </summary>
        /// <returns>An <c>Admin</c> from <c>MockData</c>.</returns>
        public static Admin GetAdmin()
        {
            return MockIUserRepository.GetMock().Object.GetAll().First(u => u is Admin) as Admin;
        }

        /// <summary>
        /// Get existing product.
        /// </summary>
        /// <returns>A product from <c>MockData</c>.</returns>
        public static Product GetExistingProduct()
        {
            return MockIProductRepository.GetMock().Object.GetAll().First();
        }
    }
}