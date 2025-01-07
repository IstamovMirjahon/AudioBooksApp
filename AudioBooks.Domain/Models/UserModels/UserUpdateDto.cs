using System.ComponentModel.DataAnnotations;
namespace AudioBooks.Domain.Models.UserModels;

public class UserUpdateDto
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
    public string? FullName { get; set; }

    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores.")]
    public string? Username { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    [Range(typeof(DateOnly), "1-1-1900", "12-31-2024", ErrorMessage = "Date of birth must be between 01-01-1900 and 12-31-2024.")]
    public DateOnly DateOfBirth { get; set; }
}
