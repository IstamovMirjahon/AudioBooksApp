using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.DTOs.Auth;

public class UserLoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [MaxLength(50, ErrorMessage = "Email can be a maximum of 50 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(255, ErrorMessage = "Password can be a maximum of 255 characters.")]
    public string Password { get; set; }
}
