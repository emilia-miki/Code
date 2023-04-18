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
    public class ProductControllerTests
    {
        private IProductController _productController;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private IAuthorizationService _authorizationService;
        private Mock<IDelegateService> _delegateServiceMock;
        private Mock<IProductAndAmountRepository> _productAndAmountRepositoryMock;
        private Mock<IPersonalInformationRepository> _personalInformationRepositoryMock;

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

            _productController = new ProductController
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
        public void SearchProductByName_ProductNotFound_ReturnsProducts()
        {
            var result = _productController.SearchProductByName("");

            _productRepositoryMock.Verify(m => m.GetByName(""), Times.Once);
            Assert.IsNull(result);
        }

        [Test]
        public void SearchProductByName_ProductFound_ReturnsProducts()
        {
            var enumerator = _productRepositoryMock.Object.GetAll().GetEnumerator();
            enumerator.MoveNext();
            var product = enumerator.Current;
            enumerator.Dispose();

            var result = _productController.SearchProductByName(product!.Name);

            _productRepositoryMock.Verify(m => m.GetByName(product!.Name), Times.Once);
            Assert.AreEqual(product, result);
        }

        [Test]
        public void ViewProducts_ReturnsAllProducts()
        {
            var result = _productController.ViewProducts();
            _productRepositoryMock.Verify(m => m.GetAll(), Times.Once);
            Assert.AreEqual(result, _productRepositoryMock.Object.GetAll());
        }

        [Test]
        public void ChangeProductInformation_AuthorizationFailed_ReturnsMessage()
        {
            var result = _productController.ChangeProductInformation(null,
                default, default, default, default, default);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void ChangeProductInformation_ProductDoesNotExist_ReturnsMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _productController.ChangeProductInformation(admin, int.MaxValue,
                default, default, default, default);

            _productRepositoryMock.Verify(m => m.GetById(int.MaxValue), Times.Once);
            Assert.AreEqual("This product does not exist", result);
        }

        [Test]
        public void ChangeProductInformation_InformationChanged_ReturnsMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _productController.ChangeProductInformation(admin, 0,
                default, default, default, default);

            var product = _productRepositoryMock.Object.GetById(0);
            Assert.AreEqual(default(int), product.Id);
            Assert.AreEqual(default(string), product.Name);
            Assert.AreEqual(default(string), product.Description);
            Assert.AreEqual(default(string), product.Category);
            Assert.AreEqual(default(decimal), product.Cost);
            Assert.AreEqual("The product information has been changed", result);
        }

        [Test]
        public void AddNewProduct_AuthorizationFailed_ReturnMessage()
        {
            var result = _productController.AddNewProduct(null,
                default, default, default, default);

            Assert.AreEqual("Authorization failed", result);
        }

        [Test]
        public void AddNewProduct_ProductAlreadyExists_ReturnMessage()
        {
            var product = TestHelper.GetExistingProduct();
            var admin = TestHelper.GetAdmin();

            var result = _productController.AddNewProduct(admin,
                product.Name, product.Category, product.Description, product.Cost);

            _productRepositoryMock.Verify(m => m.Add(It.IsAny<Product>()), Times.Never);
            Assert.AreEqual("A product with this name already exists", result);
        }

        [Test]
        public void AddNewProduct_ProductAdded_ReturnMessage()
        {
            var admin = TestHelper.GetAdmin();

            var result = _productController.AddNewProduct(admin,
                "name", "category", "description", 10);

            _productRepositoryMock.Verify(m => m.Add(It.IsAny<Product>()), Times.Once);
            Assert.AreEqual("A new product has been added successfully", result);
        }
    }
}