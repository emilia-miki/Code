namespace OnionCore
{
    /// <summary>
    /// Order status enumeration.
    /// </summary>
    public enum OrderStatus
    {
        New,
        CanceledByTheAdministrator,
        PaymentReceived,
        Sent,
        CanceledByUser,
        Received, 
        Completed
    }
}