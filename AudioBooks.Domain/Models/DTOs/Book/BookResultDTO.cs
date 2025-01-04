namespace AudioBooks.Domain.DTOs.Book;

public class BookResultDTO
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }
    public double Price { get; set; }
    public string Download_file { get; set; }
    public string Audio_file { get; set; }
    public string Image_file { get; set; }
    public  double  Rating { get; set; }
    public string Description { get; set; }
    public DateTime release_date { get; set; }
    public List<CategoryResultDTO> Categories { get; set; } = new List<CategoryResultDTO>();
    public List<CommentResultDTO> Comments { get; set; } = new List<CommentResultDTO>();
}
