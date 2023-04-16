using System.Linq;
using Moq;
using RepositoryInterface;
using OnionCore;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IOrderRepository</c> mock class.
    /// </summary>
    public static class MockIOrderRepository
    {
        /// <summary>
        /// Gets a <c>IOrderRepository</c> mock object.
        /// </summary>
        /// <returns>The mock object.</returns>
        public static Mock<IOrderRepository> GetMock()
        {
            var mock = new Mock<IOrderRepository>();

            var list = MockData.GetOrders();
            var nextId = list.Count;

            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => list.FirstOrDefault(p => p.Id == id));
            mock.Setup(m => m.GetAll()).Returns(() => list.AsEnumerable());
            mock.Setup(m => m.Add(It.IsAny<Order>()))
                .Returns((Order o) =>
                {
                    o.Id = nextId;
                    nextId++;
                    list.Add(o);
                    return o;
                });
            mock.Setup(m => m.RemoveById(It.IsAny<int>()))
                .Callback((int id) => { list.Remove(list.FirstOrDefault(o => o.Id == id)); });
            mock.Setup(m => m.GetByCustomerId(It.IsAny<int>()))
                .Returns((int id) => list.Where(o => o.Customer.Id == id));

            return mock;
        }
    }
}