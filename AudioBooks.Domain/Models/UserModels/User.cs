using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Enums;
using System.ComponentModel;

namespace AudioBooks.Domain.Models.UserModels;

public class User : BaseParametrs
{
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmAt { get; set; } = false;
    public string Password { get; set; }

    [DefaultValue(UserRole.User)]
    public UserRole Role { get; set; } = UserRole.User;
    public float? Balance { get; set; } = 0;
    public UserStatus? Status { get; set; } = UserStatus.Pending;
}
