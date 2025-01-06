using AudioBooks.Application.Service.UserServices;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Enums;
using AudioBooks.Domain.Models.DTOs.Auth;
using AudioBooks.Domain.Models.UserModels;
using AudioBooks.Infrastructure.Repositories.UserRepositories;

namespace AudioBooks.Infrastructure.Services.UserServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UserService(IUserRepository userRepository,
                       IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<SignInResponse>> SignInAsync(UserLoginDto signInDTO)
    {
        var user = await _userRepository.GetByEmailAsync(signInDTO.Email.ToLower());
        if (user is not null && !user.IsDelete && user.Password == signInDTO.Password)
        {
            string token = await _userRepository.GenerateToken(user.Id.ToString(), user.Email.ToLower(), user.Role.ToString());
            SignInResponse response = new SignInResponse()
            {
                Massage = "Kirish muvaffaqqiyatli",
                BearerToken = "Bearer " + token,
                UserId = user.Id,
                UserName = user.Email,
                Role = user.Role.ToString()
            };
            return Result<SignInResponse>.Success(response);
        }
        return Result<SignInResponse>.Failure(UserError.SignIn);
    }

    public async Task<Result<string>> SignUpService(UserRegisterDto userDTO)
    {
        var user = await _userRepository.GetByEmailAsync(userDTO.Email.ToLower());
        if (user is null || user.IsDelete)
        {
            if (await _userRepository.CheckConfirmPassword(userDTO.Password, userDTO.ConfirmPassword))
            {
                int code = await _userRepository.GenerateCode();

                await _userRepository.SendVerificationEmail(userDTO.Email.ToLower(), code);

                SingletonPattern.Instance.Id = default;
                SingletonPattern.Instance.Email = userDTO.Email.ToLower();
                SingletonPattern.Instance.Password = userDTO.Password;
                SingletonPattern.Instance.Code = code;
                SingletonPattern.Instance.CreateTime = DateTime.Now;

                return Result<string>.Success($"{userDTO.Email.ToLower()} ga tasdiqlash kodi yuborildi - {code}");
            }
            return Result<string>.Failure(UserError.ConfirmPassword);
        }
        return Result<string>.Failure(UserError.CheckEmail);
    }

    public async Task<Result<string>> VerificationCodeService(int inputCode)
    {
        if (await _userRepository.VerificationCode(inputCode, SingletonPattern.Instance.Code, SingletonPattern.Instance.CreateTime))
        {
            if (SingletonPattern.Instance.Id == default)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    FullName = SingletonPattern.Instance.FullName,
                    Email = SingletonPattern.Instance.Email,
                    Password = SingletonPattern.Instance.Password,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    IsDelete = false,
                    Role = UserRole.User
                };
                await _userRepository.AddAsync(user);

                await _unitOfWork.SaveChangesAsync();

                //await _userRepository.SendWelcomeEmail(SingletonPattern.Instance.Email, SingletonPattern.Instance.FullName);

                return Result<string>.Success("Muvaffaqqiyatli ro'yhatdan o'tdingiz");
            }
            var userResset = await _userRepository.GetByIdAsync(SingletonPattern.Instance.Id);

            userResset.Email = SingletonPattern.Instance.Email;
            userResset.Password = SingletonPattern.Instance.Password;
            userResset.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success("Muvaffaqqiyatli yangilandi");
        }
        return Result<string>.Failure(UserError.VerificationCode);
    }
    public async Task<Result<string>> RessetPasswordAsync(RessetPasswordUserDTO userRessetPassword)
    {
        var user = await _userRepository.GetByEmailAsync(userRessetPassword.Email.ToLower());
        if (user is not null && !user.IsDelete)
        {
            if (await _userRepository.CheckConfirmPassword(userRessetPassword.Password, userRessetPassword.ConfirmPassword))
            {
                int code = await _userRepository.GenerateCode();

                //await _userRepository.SendVerificationEmail(user.Email.ToLower(), code);

                SingletonPattern.Instance.Id = user.Id;
                SingletonPattern.Instance.Email = user.Email;
                SingletonPattern.Instance.Password = userRessetPassword.Password;
                SingletonPattern.Instance.Code = code;
                SingletonPattern.Instance.CreateTime = DateTime.Now;

                return Result<string>.Success($"{user.Email.ToLower()} ga tasdiqlash kodi yuborildi - {code}");
            }
            return Result<string>.Failure(UserError.ConfirmPassword);
        }
        return Result<string>.Failure(UserError.EmailNotFound);
    }

    public Task<Result<UserDto>> GetUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> UpdateUserAsync(UserUpdateDto updateUserDTO, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> ChangePasswordAsync(ChangePassword changePassword, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> DeleteUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<User>>> GetAllUsers()
    {
        throw new NotImplementedException();
    }
}
