using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.DTOs.Book;

public class CategoryUpdateDTO
{
    [Required(ErrorMessage = "Id maydoni majburiy.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name maydoni majburiy.")]
    [StringLength(100, ErrorMessage = "Name uzunligi {1} belgidan oshmasligi kerak.")]
    public string Name { get; set; }
    public string Description { get; set; }

}
