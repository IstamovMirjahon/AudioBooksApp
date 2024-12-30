using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.DTOs.Book;
public class UserLibraryCreateDTO
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid BookId { get; set; }
}
