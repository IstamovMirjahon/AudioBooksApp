using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.Models.DTOs.Auth;
using AudioBooks.Domain.Models.UserModels;

namespace AudioBooks.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<RequestResponseDto<User>> AuthenticateAsync(string email, string password);
        Task<RequestResponseDto> RegisterAsync(UserRegisterDto registerDto);

    }
}
