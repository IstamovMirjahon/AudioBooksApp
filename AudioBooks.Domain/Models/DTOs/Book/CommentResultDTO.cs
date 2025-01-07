namespace AudioBooks.Domain.DTOs.Book;

public class CommentResultDTO
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public int Value { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}
