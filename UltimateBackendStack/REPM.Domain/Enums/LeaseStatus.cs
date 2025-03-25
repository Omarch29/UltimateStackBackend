namespace REPM.Domain.Enums;

public enum LeaseStatus
{
    Pending,    // Lease is created but not yet active
    Active,     // Lease is currently in effect
    Expired,    // Lease has ended naturally
    Canceled    // Lease was terminated before the end date
    
}