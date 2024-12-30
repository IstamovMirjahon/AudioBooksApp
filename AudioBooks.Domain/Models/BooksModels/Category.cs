using AudioBooks.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.BookModels;

public class Category : BaseParametrs
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; }
    public virtual ICollection<BookCategory> BookCategories { get; set; } 
}
