using AudioBooks.Domain.Models.BookModels;
using AudioBooks.Domain.Models.UserModels;

public class UserPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual User User { get; set; }
    public virtual Category Category { get; set; }
} 