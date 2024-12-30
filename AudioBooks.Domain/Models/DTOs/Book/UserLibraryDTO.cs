namespace AudioBooks.Domain.DTOs.Book;

public class UserLibraryDTO
{
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AddedDate { get; set; }
}
