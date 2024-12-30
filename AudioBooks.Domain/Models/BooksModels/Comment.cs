using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Domain.Models.BookModels;

public class Comment
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Book Book { get; set; }
}
