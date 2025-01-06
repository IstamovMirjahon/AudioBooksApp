using AudioBooks.Domain.CustomAttributes;
using System.ComponentModel.DataAnnotations;

namespace AudioBooks.Domain.Models.UserModels;

public class ChangePassword
{
    public string OldPasswrod { get; set; }
    [Required]
    [PasswordValidation]
    public string NewPassword { get; set;}
    public string ConfirmNewPassword { get; set; }
}
