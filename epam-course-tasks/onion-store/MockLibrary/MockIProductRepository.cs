using System.Linq;
using Moq;
using RepositoryInterface;
using OnionCore;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IProductRepository</c> mock class.
    /// </summary>
    public static class MockIProductRepository
    {
        /// <summary>
        /// Gets a <c>IProductRepository</c> mock object.
        /// </summary>
        /// <returns>The mock object.</returns>
        public static Mock<IProductRepository> GetMock()
        {
            var mock = new Mock<IProductRepository>();

            var list = MockData.GetProducts();
            var nextId = list.Count;

            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => list.FirstOrDefault(p => p.Id == id));
            mock.Setup(m => m.GetAll()).Returns(() => list.AsEnumerable());
            mock.Setup(m => m.Add(It.IsAny<Product>()))
                .Returns((Product p) =>
                {
                    p.Id = nextId;
                    nextId++;
                    list.Add(p);
                    return p;
                });
            mock.Setup(m => m.RemoveById(It.IsAny<int>()))
                .Callback((int id) => { list.Remove(list.FirstOrDefault(p => p.Id == id)); });
            mock.Setup(m => m.GetByName(It.IsAny<string>()))
                .Returns((string name) => list.FirstOrDefault(p => p.Name == name));

            return mock;
        }
    }
}