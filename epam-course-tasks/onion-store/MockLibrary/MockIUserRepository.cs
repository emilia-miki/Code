using System.Linq;
using RepositoryInterface;
using Moq;
using OnionCore;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IUserRepository</c> mock class.
    /// </summary>
    public static class MockIUserRepository
    {
        /// <summary>
        /// Gets a <c>IUserRepository</c> mock object.
        /// </summary>
        /// <returns>The mock object.</returns>
        public static Mock<IUserRepository> GetMock()
        {
            var mock = new Mock<IUserRepository>();

            var list = MockData.GetUsers();
            var nextId = list.Count;

            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => list.FirstOrDefault(p => p.Id == id));
            mock.Setup(m => m.GetAll()).Returns(() => list.AsEnumerable());
            mock.Setup(m => m.Add(It.IsAny<IUser>()))
                .Returns((IUser u) =>
                {
                    u.Id = nextId;
                    nextId++;
                    list.Add(u);
                    return u;
                });
            mock.Setup(m => m.RemoveById(It.IsAny<int>()))
                .Callback((int id) => { list.Remove(list.FirstOrDefault(u => u.Id == id)); });
            mock.Setup(m => m.GetByLogin(It.IsAny<string>()))
                .Returns((string login) => list.FirstOrDefault(u => u.Login == login));
            mock.Setup(m => m.GetAllRegisteredUsers())
                .Returns(() => list.OfType<RegisteredUser>());

            return mock;
        }
    }
}