using Moq;
using ServiceInterface.IServices;

namespace MockLibrary
{
    /// <summary>
    /// A <c>IDelegateService</c> mock class.
    /// </summary>
    public static class MockIDelegateService
    {
        /// <summary>
        /// Gets a <c>IDelegateService</c> mock object. This mock uses a real instance of <c>DelegateService</c>
        /// in order to return real delegates that are required for authorization testing purposes.
        /// This mock is used for tracking method calls of the <c>DelegateService</c>.
        /// </summary>
        /// <param name="delegateService">The <c>DelegateService</c> to use.</param>
        /// <returns>The mock object.</returns>
        public static Mock<IDelegateService> GetMock(IDelegateService delegateService)
        {
            var mock = new Mock<IDelegateService>();

            mock.Setup(m => m.GuestDelegates).Returns(delegateService.GuestDelegates);
            mock.Setup(m => m.UserDelegates).Returns(delegateService.UserDelegates);
            mock.Setup(m => m.AdminDelegates).Returns(delegateService.AdminDelegates);

            return mock;
        }
    }
}