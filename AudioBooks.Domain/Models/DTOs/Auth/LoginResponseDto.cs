namespace AudioBooks.Domain.DTOs.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
