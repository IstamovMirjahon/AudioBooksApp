
namespace AudioBooks.Domain.DTOs.Book;

public class SearchPageDTO
{
    public string SearchQuery { get; set; }

    public List<BookResultDTO> Results { get; set; }
}
