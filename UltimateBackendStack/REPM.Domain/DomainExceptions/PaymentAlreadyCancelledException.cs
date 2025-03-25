namespace REPM.Domain.DomainExceptions;

public class PaymentAlreadyCancelledException : Exception
{
    private Guid PaymentId { get; }
    private static string GenerateMessage(Guid paymentId) => $"Payment with ID {paymentId} is already cancelled.";

    public PaymentAlreadyCancelledException(Guid paymentId) : base(GenerateMessage(paymentId))
    {
        PaymentId = paymentId;
    }
    
    public PaymentAlreadyCancelledException(string message) : base(message) { }
}