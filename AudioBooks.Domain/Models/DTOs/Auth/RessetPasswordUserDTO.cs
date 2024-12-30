using AudioBooks.Domain.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.DTOs.Auth;

public class RessetPasswordUserDTO
{
    [Required]
    [EmailValidation]
    public string Email { get; set; }
    [Required]
    [PasswordValidation]
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}
