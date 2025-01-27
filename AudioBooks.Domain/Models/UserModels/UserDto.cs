namespace AudioBooks.Domain.Models.UserModels;

public class UserDto
{
    public string Email { get; set; }
    public string? Fullname { get; set; }
    public string? Password { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
}
