using MockLibrary;
using ServiceInterface.Controllers;
using ServiceInterface.Services;
using NUnit.Framework;
using Moq;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;
using OnionCore;

namespace TestProject
{
    public class MainControllerTests
    {
        private IMainController _mainController;
        private Mock<IDelegateService> _delegateServiceMock;
        private Mock<IUserController> _userControllerMock;

        [SetUp]
        public void SetUp()
        {
            _delegateServiceMock = MockIDelegateService.GetMock(new DelegateService
            {
                UserController = new UserController(),
                OrderController = new OrderController(),
                ProductController = new ProductController()
            });

            _userControllerMock = new Mock<IUserController>();
            _userControllerMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() => null);
            _delegateServiceMock.Invocations.Clear();

            _mainController = new MainController(null, null, null);
            _mainController.DelegateService = _delegateServiceMock.Object;
            _mainController.UserController = _userControllerMock.Object;
        }

        [Test]
        public void GetDelegates_ArgumentNull_ReturnsGuestDelegates()
        {
            _mainController.GetDelegates(null);

            _delegateServiceMock.Verify(m => m.GuestDelegates, Times.Once);
            _userControllerMock.Verify(m => m.Login(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void GetDelegates_NoArgument_ReturnsGuestDelegates()
        {
            _mainController.GetDelegates();

            _delegateServiceMock.Verify(m => m.GuestDelegates, Times.Once);
            _userControllerMock.Verify(m => m.Login(It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public void GetDelegates_SpecifiedUser_CallsLogin()
        {
            _mainController.GetDelegates(new User
            {
                Login = "test",
                Password = "test"
            });

            _userControllerMock.Verify(m => m.Login("test", "test"), Times.Once);
        }
    }
}