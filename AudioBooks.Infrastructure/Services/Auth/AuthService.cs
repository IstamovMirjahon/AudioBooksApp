using AudioBooks.Application.Interfaces.Auth;
using AudioBooks.Application.Interfaces.Email;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.Enums;
using AudioBooks.Domain.Models.DTOs.Auth;
using AudioBooks.Domain.Models.UserModels;
using AudioBooks.Infrastructure.Repositories.UserRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace AudioBooks.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _redis;

    public AuthService(IUserRepository userRepository,
                       IPasswordHasher<User> passwordHasher, 
                       ApplicationDbContext context, 
                       IEmailService emailService, 
                       IUnitOfWork unitOfWork,
                       IDistributedCache redis)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _context = context;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _redis = redis;
    }

    public async Task<RequestResponseDto<User>> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLower());

        if (user == null)
        {
            return new RequestResponseDto<User>()
            {
                Success = false,
                Message = "Email or Password incorrect",
                Data = null
            };
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

        if (result == PasswordVerificationResult.Success)
        {
            return new RequestResponseDto<User>()
            {
                Success = true,
                Message = "Authentication successful",
                Data = user 
            };
        }

        return new RequestResponseDto<User>()
        {
            Success = false,
            Message = "Email or Password incorrect",
            Data = null
        };
    }

    public async Task<RequestResponseDto> RegisterAsync(UserRegisterDto registerDto)
    {
        // Email mavjudligini tekshirish
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email.ToLower() == registerDto.Email.ToLower());

        if (existingUser != null)
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = "Ushbu email allaqachon ro'yxatdan o'tgan."
            };
        }
       

        // Foydalanuvchi ma'lumotlarini Redis uchun tayyorlash
        var userTemp = new
        {
            Email = registerDto.Email.ToLower(),
            Password = _passwordHasher.HashPassword(new User(), registerDto.Password),
            Role = UserRole.User,
            BirthDate = registerDto.DateOfBirth
        };
        var redisKey = $"temp_user:{registerDto.Email.ToLower()}";

        // Redis'ga foydalanuvchini vaqtinchalik saqlash
        await _redis.SetStringAsync(redisKey, JsonConvert.SerializeObject(userTemp), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) // 15 daqiqa amal qilish muddati
        });

        // Tasdiqlash kodi yaratish
        var verificationCode = _emailService.GenerateVerificationCode();

        // Tasdiqlash kodini Redis’ga yozish
        await _redis.SetStringAsync($"verification:{registerDto.Email.ToLower()}", verificationCode, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
        });

        // Email yuborish
        await _emailService.SendVerificationEmailAsync(registerDto.Email, verificationCode);

        return new RequestResponseDto
        {
            Success = true,
            Message = "Ro'yxatdan muvaffaqiyatli o'tildi. Email tasdiqlash kodi yuborildi."
        };
    }
}