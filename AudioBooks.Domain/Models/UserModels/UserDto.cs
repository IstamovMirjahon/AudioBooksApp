using AudioBooks.Domain.Enums;

namespace AudioBooks.Domain.Models.UserModels;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string? Fullname { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public bool EmailConfirmAt { get; set; }
    public string Role { get; set; }
    public float? Balance { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserStatus? Status { get; set; }
}
