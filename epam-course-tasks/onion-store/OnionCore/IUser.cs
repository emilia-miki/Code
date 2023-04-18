namespace OnionCore
{
    /// <summary>
    /// User model interface.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// User login.
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// User password.
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Personal information of the customer.
        /// </summary>
        public PersonalInformation PersonalInformation { get; set; }
    }
}