using AudioBooks.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.UserModels;

public class UserCreateDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string Password { get; set; }

    public UserRole Role { get; set; } = UserRole.User;
}

