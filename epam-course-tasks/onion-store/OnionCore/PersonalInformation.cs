namespace OnionCore
{
    /// <summary>
    /// Personal information class.
    /// </summary>
    public class PersonalInformation
    {
        /// <summary>
        /// Personal information entry ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The customer's name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The customer's surname.
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// The customer's email.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The customer's phone number.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}