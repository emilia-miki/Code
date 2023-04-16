using RepositoryInterface;
using ServiceInterface.IServices;

namespace ServiceInterface.IControllers
{
    /// <summary>
    /// Controller interface.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// A repository of <c>Product</c> entries.
        /// </summary>
        public IProductRepository ProductRepository { get; set; }
        
        /// <summary>
        /// A repository of <c>ProductAndAmount</c> entries.
        /// </summary>
        public IProductAndAmountRepository ProductAndAmountRepository { get; set; }
        
        /// <summary>
        /// A repository of <c>PersonalInformation</c> entries.
        /// </summary>
        public IPersonalInformationRepository PersonalInformationRepository { get; set; }

        /// <summary>
        /// A repository of <c>Order</c> entries.
        /// </summary>
        public IOrderRepository OrderRepository { get; set; }
        
        /// <summary>
        /// A repository of <c>IUser</c> entries.
        /// </summary>
        public IUserRepository UserRepository { get; set; }

        /// <summary>
        /// A <c>IDelegateService</c> instance.
        /// </summary>
        public IDelegateService DelegateService { get; set; }
        
        /// <summary>
        /// A <c>IAuthorizationService</c> instance.
        /// </summary>
        public IAuthorizationService AuthorizationService { get; set; }
    }
}