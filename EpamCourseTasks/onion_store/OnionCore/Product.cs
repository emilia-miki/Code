namespace OnionCore
{
    /// <summary>
    /// Product class.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Product name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Product category.
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Product description.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Product cost.
        /// </summary>
        public decimal Cost { get; set; }
    }
}