using System.Linq;
using Moq;
using OnionCore;
using RepositoryInterface;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IPersonalInformationRepository</c> mock class.
    /// </summary>
    public static class MockIPersonalInformationRepository
    {
        /// <summary>
        /// Gets a <c>IPersonalInformationRepository</c> mock object.
        /// </summary>
        /// <returns>The mock object.</returns>
        public static Mock<IPersonalInformationRepository> GetMock()
        {
            var mock = new Mock<IPersonalInformationRepository>();

            var list = MockData.GetPersonalInformation();
            var nextId = list.Count;
            
            mock.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns((int id) => list.FirstOrDefault(pi => pi.Id == id));
            mock.Setup(m => m.GetAll()).Returns(() => list.AsEnumerable());
            mock.Setup(m => m.Add(It.IsAny<PersonalInformation>()))
                .Returns((PersonalInformation pi) =>
                {
                    pi.Id = nextId;
                    nextId++;
                    list.Add(pi);
                    return pi;
                });
            mock.Setup(m => m.RemoveById(It.IsAny<int>()))
                .Callback((int id) => { list.Remove(list.FirstOrDefault(pi => pi.Id == id)); });

            return mock;
        }
    }
}