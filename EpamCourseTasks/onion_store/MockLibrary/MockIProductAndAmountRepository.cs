using System.Linq;
using Moq;
using OnionCore;
using RepositoryInterface;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IProductAndAmountRepository</c> mock class.
    /// </summary>
    public static class MockIProductAndAmountRepository
    {
        /// <summary>
        /// Gets a <c>IProductAndAmountRepository</c> mock object.
        /// </summary>
        /// <returns>The mock object.</returns>
        public static Mock<IProductAndAmountRepository> GetMock()
        {
            var mock = new Mock<IProductAndAmountRepository>();

            var list = MockData.GetProductList();
            var nextId = list.Count;
            
            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => list.FirstOrDefault(p => p.Id == id));
            mock.Setup(m => m.GetAll()).Returns(() => list.AsEnumerable());
            mock.Setup(m => m.Add(It.IsAny<ProductAndAmount>()))
                .Returns((ProductAndAmount pa) =>
                {
                    pa.Id = nextId;
                    nextId++;
                    list.Add(pa);
                    return pa;
                });
            mock.Setup(m => m.RemoveById(It.IsAny<int>()))
                .Callback((int id) => { list.Remove(list.FirstOrDefault(pa => pa.Id == id)); });

            return mock;
        }
    }
}