using System.Collections.Generic;
using System.Reflection;
using RepositoryInterface;
using OnionCore;
using ServiceInterface.IControllers;
using ServiceInterface.IServices;

namespace ServiceInterface.Controllers
{
    /// <summary>
    /// <c>Product</c> controller. Implements <c>IProductController</c>.
    /// </summary>
    public class ProductController : IProductController
    {
        /// <inheritdoc cref="IController.ProductRepository"/>
        public IProductRepository ProductRepository { get; set; }

        /// <inheritdoc cref="IController.ProductAndAmountRepository"/>
        public IProductAndAmountRepository ProductAndAmountRepository { get; set; }

        /// <inheritdoc cref="IController.PersonalInformationRepository"/>
        public IPersonalInformationRepository PersonalInformationRepository { get; set; }

        /// <inheritdoc cref="IController.OrderRepository"/>
        public IOrderRepository OrderRepository { get; set; }
        
        /// <inheritdoc cref="IController.UserRepository"/>
        public IUserRepository UserRepository { get; set; }
        
        /// <inheritdoc cref="IController.DelegateService"/>
        public IDelegateService DelegateService { get; set; }
        
        /// <inheritdoc cref="IController.AuthorizationService"/>
        public IAuthorizationService AuthorizationService { get; set; }
        
        /// <inheritdoc cref="IProductController.SearchProductByName"/>
        [Command("searchProductName")]
        [RolePermissions(Role.Guest, Role.RegisteredUser, Role.Admin)]
        public Product SearchProductByName(string name)
        {
            return ProductRepository.GetByName(name);
        }
        
        /// <inheritdoc cref="IProductController.ViewProducts"/>
        [Command("viewProducts")]
        [RolePermissions(Role.Guest, Role.RegisteredUser, Role.Admin)]
        public IEnumerable<Product> ViewProducts()
        {
            return ProductRepository.GetAll();
        }

        /// <summary>
        /// Change <c>Product</c> information.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="productId">ID of the <c>Product</c>.</param>
        /// <param name="name">New name.</param>
        /// <param name="category">New category.</param>
        /// <param name="description">New description.</param>
        /// <param name="cost">New cost.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "This product does not exist",
        /// "The product information has been changed".
        /// </returns>
        [Command("changeProductInformation")]
        [RolePermissions(Role.Admin)]
        public string ChangeProductInformation(IUser argUser, int productId, string name, string category,
            string description, decimal cost)
        {
            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            var product = ProductRepository.GetById(productId);
            if (product == default)
            {
                return "This product does not exist";
            }

            product.Name = name;
            product.Category = category;
            product.Description = description;
            product.Cost = cost;
            return "The product information has been changed";
        }

        /// <summary>
        /// Add new <c>Product</c>.
        /// </summary>
        /// <param name="argUser">The <c>IUser</c> who called the method.</param>
        /// <param name="name">Name.</param>
        /// <param name="category">Category.</param>
        /// <param name="description">Description.</param>
        /// <param name="cost">Cost.</param>
        /// <returns>
        /// String description of the result of method execution:
        /// "Authorization failed",
        /// "A product with this name already exists",
        /// "A new product has been added successfully".
        /// </returns>
        [Command("addNewProduct")]
        [RolePermissions(Role.Admin)]
        public string AddNewProduct(IUser argUser, string name, string category, string description, decimal cost)
        {

            var attribute = (RolePermissionsAttribute) MethodBase.GetCurrentMethod()!
                .GetCustomAttribute(typeof(RolePermissionsAttribute));
            if (!AuthorizationService.Authorize(argUser, attribute!.Roles))
            {
                return "Authorization failed";
            }

            if (ProductRepository.GetByName(name) != default)
            {
                return "A product with this name already exists";
            }

            var product = new Product
            {
                Name = name,
                Category = category,
                Description = description,
                Cost = cost
            };

            ProductRepository.Add(product);
            return "A new product has been added successfully";
        }
    }
}