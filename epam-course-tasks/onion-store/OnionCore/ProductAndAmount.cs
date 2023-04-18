namespace OnionCore
{
    /// <summary>
    /// Product and amount entry class.
    /// </summary>
    public class ProductAndAmount
    {
        /// <summary>
        /// Product and amount entry ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Product ID.
        /// </summary>
        public Product Product { get; set; }
        
        /// <summary>
        /// Amount of the product.
        /// </summary>
        public int Amount { get; set; }
    }
}