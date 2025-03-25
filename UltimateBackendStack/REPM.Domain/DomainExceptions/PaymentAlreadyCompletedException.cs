namespace REPM.Domain.DomainExceptions;

public class PaymentAlreadyCompletedException: Exception
{
    private Guid PaymentId { get; }
    private static string GenerateMessage(Guid paymentId) => $"Payment with ID {paymentId} is already completed.";

    public PaymentAlreadyCompletedException(Guid paymentId) : base(GenerateMessage(paymentId))
    {
        PaymentId = paymentId;
    }
    
    public PaymentAlreadyCompletedException(string message) : base(message) { }
    
}