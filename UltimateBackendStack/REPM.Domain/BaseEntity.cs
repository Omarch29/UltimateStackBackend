namespace REPM.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public void SetUpdatedAt(DateTime dateTime)
    {
        UpdatedAt = dateTime;
    }
    
    public void SetCreatedAt(DateTime dateTime)
    {
        CreatedAt = dateTime;
    }
    
    public virtual void Delete()
    {
        IsDeleted = true;
    }
    
    public virtual void Restore()
    {
        IsDeleted = false;
    }
}