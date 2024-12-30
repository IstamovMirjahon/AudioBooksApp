namespace AudioBooks.Domain.Models.BookModels;

public class BookCategory 
{
    public Guid BookId { get; set; }
    public virtual Book Book { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; }
}
