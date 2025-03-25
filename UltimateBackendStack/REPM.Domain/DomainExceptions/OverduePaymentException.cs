namespace REPM.Domain.DomainExceptions;

public class OverduePaymentException : Exception
{
    private Guid RenterId { get; }
    private static string GenerateMessage(Guid renterId) => $"Renter with ID {renterId} has overdue payments.";

    public OverduePaymentException(Guid renterId) : base(GenerateMessage(renterId))
    {
        RenterId = renterId;
    }
    
    public OverduePaymentException(string message) : base(message) { }
}