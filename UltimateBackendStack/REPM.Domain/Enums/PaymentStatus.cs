namespace REPM.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,   // Payment is expected but not yet received
    Completed = 1, // Payment has been successfully received
    Failed = 2,    // Payment attempt failed (e.g., insufficient funds)
    Overdue = 3,   // Payment was not received by the due date
    Canceled = 4   // Payment was canceled by the user or system
}

public static class PaymentStatusExtensions
{
    /// <summary>
    /// Determines if a payment can transition to the specified status.
    /// </summary>
    public static bool CanTransitionTo(this PaymentStatus currentStatus, PaymentStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (PaymentStatus.Pending, PaymentStatus.Completed) => true, // ✅ Can be completed
            (PaymentStatus.Pending, PaymentStatus.Failed) => true,    // ✅ Can fail
            (PaymentStatus.Pending, PaymentStatus.Overdue) => true,   // ✅ Can become overdue
            (PaymentStatus.Pending, PaymentStatus.Canceled) => true,  // ✅ Can be canceled

            (PaymentStatus.Failed, PaymentStatus.Pending) => true,    // ✅ Retry failed payment
            (PaymentStatus.Failed, PaymentStatus.Canceled) => true,   // ✅ Cancel after failure

            (PaymentStatus.Overdue, PaymentStatus.Completed) => true, // ✅ Overdue can be completed if paid
            (PaymentStatus.Overdue, PaymentStatus.Canceled) => true,  // ✅ Overdue can be canceled

            _ => false // ❌ Any other transition is invalid
        };
    }

    /// <summary>
    /// Checks if a payment is in a final state.
    /// </summary>
    public static bool IsFinal(this PaymentStatus status)
    {
        return status is PaymentStatus.Completed or PaymentStatus.Canceled;
    }

    /// <summary>
    /// Gets a human-friendly description of the payment status.
    /// </summary>
    public static string GetDescription(this PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Pending => "Payment is pending.",
            PaymentStatus.Completed => "Payment has been completed.",
            PaymentStatus.Failed => "Payment failed. Please try again.",
            PaymentStatus.Overdue => "Payment is overdue!",
            PaymentStatus.Canceled => "Payment has been canceled.",
            _ => "Unknown status."
        };
    }
}