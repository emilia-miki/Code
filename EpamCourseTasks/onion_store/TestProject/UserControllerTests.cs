using System;
using System.Linq;
using ServiceInterface.Controllers;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;
using ServiceInterface.Services;
using RepositoryInterface;
using MockLibrary;
using Moq;
using NUnit.Framework;
using OnionCore;

namespace TestProject
{
    public class UserControllerTests
    {
        private IUserController _userController;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IProductAndAmountRepository> _productAndAmountRepositoryMock;
        private Mock<IPersonalInformationRepository> _personalInformationRepositoryMock;
        private IAuthorizationService _authorizationService;
        private Mock<IDelegateService> _delegateServiceMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = MockIUserRepository.GetMock();
            _orderRepositoryMock = MockIOrderRepository.GetMock();
            _productRepositoryMock = MockIProductRepository.GetMock();
            _productAndAmountRepositoryMock = MockIProductAndAmountRepository.GetMock();
            _personalInformationRepositoryMock = MockIPersonalInformationRepository.GetMock();
            _delegateServiceMock = MockIDelegateService.GetMock(new DelegateService
            {
                UserController = new UserController(),
                ProductController = new ProductController(),
                OrderController = new OrderController()
            });
            _authorizationService = new AuthorizationService();
            _authorizationService.UserRepository = _userRepositoryMock.Object;
            _authorizationService.DelegateService = _delegateServiceMock.Object;

            _userController = new UserController
            {
                AuthorizationService = _authorizationService,
                DelegateService = _delegateServiceMock.Object,
                OrderRepository = _orderRepositoryMock.Object,
                ProductRepository = _productRepositoryMock.Object,
                UserRepository = _userRepositoryMock.Object,
                ProductAndAmountRepository = _productAndAmountRepositoryMock.Object,
                PersonalInformationRepository = _personalInformationRepositoryMock.Object
            };
        }

        [Test]
        public void Register_LoginTaken_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _userController.Register(user.Login, user.Password);

            _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Once);
            Assert.AreEqual("This login is already taken", result);
        }

        [Test]
        public void Register_Success_ReturnMessage()
        {
            var result = _userController.Register("newUser", "newUserPassword");

            _userRepositoryMock.Verify(m => m.Add(It.IsAny<RegisteredUser>()));
            Assert.AreEqual("You have been registered successfully", result);
        }

        [Test]
        public void Login_NoLogin_ReturnGuestDelegates()
        {
            var result = _userController.Login(null, null);

            _userRepositoryMock.Verify(m => m.GetByLogin(null), Times.Once);
            _delegateServiceMock.Verify(m => m.GuestDelegates, Times.Once);
            Assert.AreEqual(_delegateServiceMock.Object.GuestDelegates, result);
        }

        [Test]
        public void Login_WrongPassword_ReturnGuestDelegates()
        {
            var user = TestHelper.GetRegisteredUser();
            user.Password = "";

            var result = _userController.Login(user.Login, user.Password);

            _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Once);
            _delegateServiceMock.Verify(m => m.GuestDelegates, Times.Once);
            Assert.AreEqual(_delegateServiceMock.Object.GuestDelegates, result);
        }

        [Test]
        public void Login_Admin_ReturnAdminDelegates()
        {
            var admin = TestHelper.GetAdmin();

            var result = _userController.Login(admin.Login, admin.Password);

            _userRepositoryMock.Verify(m => m.GetByLogin(admin.Login), Times.Once);
            _delegateServiceMock.Verify(m => m.AdminDelegates, Times.Once);
            Assert.AreEqual(_delegateServiceMock.Object.AdminDelegates, result);
        }

        [Test]
        public void Login_RegisteredUser_ReturnAdminDelegates()
        {
            var user = TestHelper.GetAdmin();

            var result = _userController.Login(user.Login, user.Password);

            _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Once);
            _delegateServiceMock.Verify(m => m.AdminDelegates, Times.Once);
            Assert.AreEqual(_delegateServiceMock.Object.AdminDelegates, result);
        }

        [Test]
        public void SignOut_ReturnGuestDelegates()
        {
            var result = _userController.SignOut();

            _delegateServiceMock.Verify(m => m.GuestDelegates, Times.Once);
            Assert.AreEqual(_delegateServiceMock.Object.GuestDelegates, result);
        }

        [Test]
        public void ViewUsersPersonalInformation_AuthorizationFailed_ReturnEmptyList()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _userController.ViewUsersPersonalInformation(user);

            _userRepositoryMock.Verify(m => m.GetAllRegisteredUsers(), Times.Never);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ViewUsersPersonalInformation_AdminAuthorized_ReturnUsersPersonalInformation()
        {
            var admin = TestHelper.GetAdmin();

            var result = _userController.ViewUsersPersonalInformation(admin);

            _userRepositoryMock.Verify(m => m.GetAllRegisteredUsers(), Times.Once);
            Assert.AreEqual(_userRepositoryMock.Object.GetAllRegisteredUsers().Select(u => 
                new Tuple<int, string, PersonalInformation>(u.Id, u.Login, 
                    _personalInformationRepositoryMock.Object.GetById(u.Id))), result);
        }

        [Test]
        public void ChangePersonalInformation_AuthorizationFailed_ReturnMessage()
        {
            var result = _userController.ChangePersonalInformation(null, null);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void ChangePersonalInformation_SuccessfullyChanged_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();
            var id = user.Id;

            var result = _userController.ChangePersonalInformation(user,
                new PersonalInformation
                {
                    Name = "newName"
                });

            _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Exactly(2));
            var newUser = _userRepositoryMock.Object.GetById(id) as RegisteredUser;
            Assert.AreEqual("newName", newUser!.PersonalInformation.Name);
            Assert.AreEqual("Profile information has been successfully changed", result);
        }

        [Test]
        public void ChangeUserPersonalInformation_AuthorizationFailed_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _userController.ChangeUserPersonalInformation(user,
                default, default);

            _userRepositoryMock.Verify(m => m.GetById(default), Times.Never);
            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void ChangeUserPersonalInformation_NullArgument_ReturnMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _userController.ChangeUserPersonalInformation(admin,
                0, null);

            _userRepositoryMock.Verify(m => m.GetById(default), Times.Once);
            Assert.AreEqual("Personal information is null", result);
        }

        [Test]
        public void ChangeUserPersonalInformation_UserDoesNotExist_ReturnMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _userController.ChangeUserPersonalInformation(admin,
                int.MaxValue, default);

            _userRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("The specified user doesn't exist", result);
        }

        [Test]
        public void ChangeUserPersonalInformation_ChangedSuccessfully_ReturnMessage()
        {
            var admin = TestHelper.GetAdmin();
            var user = TestHelper.GetRegisteredUser();
            var id = user.Id;

            var result = _userController.ChangeUserPersonalInformation(admin,
                id, new PersonalInformation
                {
                    Name = "newName",
                    Surname = "newSurname"
                });

            _userRepositoryMock.Verify(m => m.GetById(id), Times.Once);
            user = _userRepositoryMock.Object.GetById(user.Id) as RegisteredUser;
            Assert.AreEqual("newName", user!.PersonalInformation.Name);
            Assert.AreEqual("newSurname", user!.PersonalInformation.Surname);
            Assert.AreEqual("User personal information has been changed successfully", result);
        }

        [Test]
        public void ChangeCredentials_AuthorizationFailed_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();
            user.Password = "";

            var result = _userController.ChangeCredentials(user, "", "");
            
            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void ChangeCredentials_ChangedSuccessfully_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _userController.ChangeCredentials(user, "newLogin", "newPassword");
            
            Assert.AreEqual("newLogin", _userRepositoryMock.Object.GetById(user.Id).Login);
            Assert.AreEqual("newPassword", _userRepositoryMock.Object.GetById(user.Id).Password);
            Assert.AreEqual("Credentials have been successfully changed", result);
        }
    }
}