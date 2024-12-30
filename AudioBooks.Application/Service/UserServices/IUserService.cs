using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Models.DTOs.Auth;
using AudioBooks.Domain.Models.UserModels;

namespace AudioBooks.Application.Service.UserServices;

public interface IUserService
{
    Task<Result<string>> SignUpService(UserRegisterDto userDTO);
    Task<Result<SignInResponse>> SignInAsync(UserLoginDto signInDTO);
    //Task<Result<string>> RessetPasswordAsync(RessetPasswordUserDTO userRessetPassword);
    Task<Result<string>> VerificationCodeService(int code);
    //Task<Result<List<GetUser>>> GetUserAsyncs();
    //Task<Result<string>> UpdateUserAsync(UpdateUserDTO updateUserDTO, string token);
    //Task<Result<string>> ChangePasswordAsync(ChangePassword changePassword, string token);
    //Task<Result<string>> DeleteUserAsync(string token);
}
