using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Models.DTOs.Auth;
using AudioBooks.Domain.Models.UserModels;

namespace AudioBooks.Application.Service.UserServices;

public interface IUserService
{
    Task<Result<string>> SignUpService(UserRegisterDto userDTO);
    Task<Result<SignInResponse>> SignInAsync(UserLoginDto signInDTO);
    Task<Result<string>> RessetPasswordAsync(RessetPasswordUserDTO userRessetPassword);
    Task<Result<string>> VerificationCodeService(int code);
    Task<Result<UserDto>> GetUserAsync(Guid id);
    Task<Result<List<User>>> GetAllUsers();
    Task<Result<string>> UpdateUserAsync(UserUpdateDto updateUserDTO, Guid id);
    Task<Result<string>> ChangePasswordAsync(ChangePassword changePassword, Guid id);
    Task<Result<string>> DeleteUserAsync(Guid id);
}
