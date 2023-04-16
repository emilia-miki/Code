using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ServiceInterface.Controllers;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;
using RepositoryInterface;
using MockLibrary;
using ServiceInterface.Services;
using OnionCore;

namespace TestProject
{
    public class OrderControllerTests
    {
        private IOrderController _orderController;
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
                OrderController = new OrderController(),
                ProductController = new ProductController(),
                UserController = new UserController()
            });
            _authorizationService = new AuthorizationService();
            _authorizationService.DelegateService = _delegateServiceMock.Object;
            _authorizationService.UserRepository = _userRepositoryMock.Object;

            _orderController = new OrderController
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
        public void CreateOrder_AuthorizationFailed_ReturnMessage()
        {
            var result = _orderController.CreateOrder(null, null);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void CreateOrder_ProductDoesNotExist_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.CreateOrder(user, new List<ProductAndAmount> {
                new ProductAndAmount
                {
                    Product = new Product
                    {
                        Id = int.MaxValue
                    },
                    Amount = 1
                }
            });

            Assert.AreEqual("This product does not exist", result);
        }

        [Test]
        public void CreateOrder_OrderCreated_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.CreateOrder(user, new List<ProductAndAmount>
            {
                new ProductAndAmount
                {
                    Product = _productRepositoryMock.Object.GetById(0),
                    Amount = 2
                }
            });

            _orderRepositoryMock.Verify(m => m.Add(It.IsAny<Order>()), Times.Once);
            Assert.AreEqual("Order created successfully", result);
        }

        [Test]
        public void PayOrder_AuthorizationFailed_ReturnMessage()
        {
            var result = _orderController.PayOrder(null, 0);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void PayOrder_OrderDoesNotExist_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.PayOrder(user, int.MaxValue);

            _orderRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("This order doesn't exist", result);
        }

        [Test]
        public void PayOrder_PaymentReceived_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.PayOrder(user, 0);

            _orderRepositoryMock.Verify(m => m.GetById(0), Times.Once);
            Assert.AreEqual(OrderStatus.PaymentReceived, _orderRepositoryMock.Object.GetById(0).Status);
            Assert.AreEqual("Payment received", result);
        }

        [Test]
        public void Cancel_AuthorizationFailed_ReturnMessage()
        {
            var result = _orderController.CancelOrder(null, 0);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void Cancel_OrderDoesNotExist_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.CancelOrder(user, int.MaxValue);

            _orderRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("This order doesn't exist", result);
        }

        [Test]
        public void Cancel_WrongUser_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.CancelOrder(user, 0);

            Assert.AreEqual("You can only cancel your own orders", result);
        }

        [Test]
        public void Cancel_OrderReceived_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var originalStatus = _orderController.OrderRepository.GetById(1).Status;
            _orderController.OrderRepository.GetById(1).Status = OrderStatus.Received;
            var result = _orderController.CancelOrder(user, 1);
            Assert.AreEqual("You can't cancel an order that has already been received", result);

            _orderController.OrderRepository.GetById(1).Status = OrderStatus.Completed;
            result = _orderController.CancelOrder(user, 1);
            Assert.AreEqual("You can't cancel an order that has already been received", result);
            _orderController.OrderRepository.GetById(1).Status = originalStatus;
        }

        [Test]
        public void Cancel_OrderCanceled_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.CancelOrder(user, 1);

            _orderRepositoryMock.Verify(m => m.GetById(1), Times.Once);
            Assert.AreEqual(OrderStatus.CanceledByUser, _orderRepositoryMock.Object.GetById(1).Status);
            Assert.AreEqual("The order has been successfully canceled", result);
        }

        [Test]
        public void ReviewOrderHistory_AuthorizationFailed_ReturnEmptyList()
        {
            var admin = TestHelper.GetAdmin();

            var result = _orderController.ReviewOrderHistory(admin);

            Assert.IsEmpty(result);
        }

        [Test]
        public void ReviewOrderHistory_Authorized_ReturnCorrectOrders()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.ReviewOrderHistory(user);

            _userRepositoryMock.Verify(m => m.GetByLogin(user.Login), Times.Exactly(2));
            _orderRepositoryMock.Verify(m => m.GetByCustomerId(user.Id), Times.Once);
            Assert.AreEqual(_orderRepositoryMock.Object.GetByCustomerId(user.Id), result);
        }

        [Test]
        public void SetOrderStatusReceived_AuthorizationFailed_ReturnMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _orderController.SetOrderStatusReceived(admin, 0);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void SetOrderStatusReceived_OrderDoesNotExist_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.SetOrderStatusReceived(user, int.MaxValue);

            _orderRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("This order doesn't exist", result);
        }

        [Test]
        public void SetOrderStatusReceived_SuccessfullySet_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();

            var result = _orderController.SetOrderStatusReceived(user, 0);

            _orderRepositoryMock.Verify(m => m.GetById(0), Times.Once);
            Assert.AreEqual(OrderStatus.Received, _orderRepositoryMock.Object.GetById(0).Status);
            Assert.AreEqual("Order status successfully set to Received", result);
        }

        [Test]
        public void ChangeOrderStatus_AuthorizationFailed_ReturnMessage()
        {
            var user = TestHelper.GetRegisteredUser();
            var result = _orderController.ChangeOrderStatus(user, 0, OrderStatus.Completed);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void ChangeOrderStatus_OrderDoesNotExit_ReturnMessage()
        {
            var user = TestHelper.GetAdmin();

            var result = _orderController.ChangeOrderStatus(user, int.MaxValue, OrderStatus.Completed);

            _orderRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("This order does not exist", result);
        }

        private void CheckStatusChange(OrderStatus original, OrderStatus changed, bool success)
        {
            var admin = TestHelper.GetAdmin();
            _orderRepositoryMock.Object.GetById(1).Status = original;
            _orderRepositoryMock.Invocations.Clear();
            
            var result = _orderController.ChangeOrderStatus(admin, 1, changed);
            
            _orderRepositoryMock.Verify(m => m.GetById(1), Times.Once);
            Assert.AreEqual(success ? changed : original, _orderRepositoryMock.Object.GetById(1).Status);
            Assert.AreEqual(
                success ? "The order status has been changed" : "This status change is illegal", 
                result
                );
        }

        [Test]
        public void ChangeOrderStatus_StatusChange_ReturnMessage()
        {
            foreach (OrderStatus i in Enum.GetValues(typeof(OrderStatus)))
            {
                foreach (OrderStatus j in Enum.GetValues(typeof(OrderStatus)))
                {
                    var success = OrderController.IsOrderStatusChangePermitted(i, j);
                    CheckStatusChange(i, j, success);
                }
            }
        }
    }
}