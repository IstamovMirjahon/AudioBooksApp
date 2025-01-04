using AudioBooks.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.BookModels;

public class Book : BaseParametrs
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
    public string Author { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public double Price { get; set; }

    public string ImageFile { get; set; } 
    public string DownloadFile { get; set; }
    public string AudioFile { get; set; } 

    [Required(ErrorMessage = "Release date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    [CustomValidation(typeof(Book), nameof(ValidateReleaseDate))]
    public DateTime ReleaseDate { get; set; }
    public double Rating { get; set; }

    public virtual ICollection<BookCategory> BookCategories { get; set; }
    public virtual ICollection<Comment> Comments { get; set; }

    public static ValidationResult ValidateReleaseDate(DateTime releaseDate, ValidationContext context)
    {
        if (releaseDate > DateTime.UtcNow)
        {
            return new ValidationResult("Release date cannot be in the future.");
        }
        return ValidationResult.Success;
    }
}
