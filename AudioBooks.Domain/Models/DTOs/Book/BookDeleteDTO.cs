using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.DTOs.Book;

public class BookDeleteDTO
{
    [Required(ErrorMessage = "Id maydoni majburiy.")]
    public Guid Id { get; set; }
}
