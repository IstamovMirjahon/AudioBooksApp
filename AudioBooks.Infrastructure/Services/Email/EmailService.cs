using AudioBooks.Application.Interfaces.Email;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.Models.UserModels;
using AudioBooks.Domain.StaticModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;

namespace AudioBooks.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _redis;

    public EmailService(IOptions<SmtpSettings> smtpSettings, 
                        ApplicationDbContext context, 
                        IPasswordHasher<User> passwordHasher,
                        IUnitOfWork unitOfWork,
                        IDistributedCache cache)
    {
        _context = context;
        _smtpSettings = smtpSettings.Value;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _redis = cache;
    }

    public async Task<RequestResponseDto> SendEmailAsync(string toEmail, string subject, string message)
    {
        try
        {
            var smtpClient = new SmtpClient(_smtpSettings.Server) // Portni o'zgartirish
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);


            await smtpClient.SendMailAsync(mailMessage);

            return new RequestResponseDto
            {
                Success = true,
                Message = "Xabar muvaffaqiyatli yuborildi!"
            };
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP xatosi: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

            return new RequestResponseDto
            {
                Success = false,
                Message = $"SMTP xatosi: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = $"Umumiy xato: {ex.Message}"
            };
        }


    }
    public async Task<RequestResponseDto> SendWelcomeEmailAsync(string toEmail, string password)
    {
        try
        {
            var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                Subject = "Xush kelibsiz",
                Body = EmailTemplate.GetWelcomeEmailTemplate(toEmail, password),
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

            return new RequestResponseDto
            {
                Success = true,
                Message = "Xabar muvaffaqiyatli yuborildi!"
            };

        }
        catch
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = $"Xabarni yuborishda xatolik!"
            };
        }
    }

    public async Task<RequestResponseDto> SendVerificationEmailAsync(string toEmail, string verificationCode)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == toEmail.ToLower());

            if (user != null)
            {
                return new RequestResponseDto
                {
                    Success = false,
                    Message = "Email tizimda mavjud!"
                };
            }

            // So'nggi yuborilgan emailni qidirish
            // Redis'dan so'nggi yuborilgan kodni tekshirish
            var redisKey = $"verification:{toEmail.ToLower()}";
            //var lastSentCode = await _redis.GetStringAsync(redisKey);

            var redisConnection = ConnectionMultiplexer.Connect("localhost:6379"); // Redis ulanishi
            var redisDatabase = redisConnection.GetDatabase();

            //var ttl = await redisDatabase.KeyTimeToLiveAsync(redisKey);
            //if (ttl.HasValue && ttl.Value.TotalSeconds > 0)
            //{
            //    return new RequestResponseDto
            //    {
            //        Success = false,
            //        Message = $"Kodni qayta yuborish uchun {Math.Round(ttl.Value.TotalSeconds)} soniya kutishingiz kerak"
            //    };
            //}

            // SMTP Clientni "using" bilan ochish
            using (var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = "Tasdiqlash Kodingiz",
                    //Body = EmailTemplate.GetVerificationEmailTemplate(toEmail, verificationCode),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }

            return new RequestResponseDto
            {
                Success = true,
                Message = "Tasdiqlash xabari muvaffaqiyatli yuborildi!"
            };
        }
        catch (Exception ex)
        {
            // Xatolikni loglash yoki tekshirish uchun xatolikni batafsil olish
            return new RequestResponseDto
            {
                Success = false,
                Message = $"Tasdiqlash xabarini yuborishda xatolik: {ex.Message}"
            };
        }
    }


    public async Task<RequestResponseDto> VerifyEmailAsync(string email,string verificationCode)
    {
        var redisKey = $"verification:{email.ToLower()}";
        var storedCode = await _redis.GetStringAsync(redisKey);

        if (storedCode == null)
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = "Tasdiqlash kodi eskirgan yoki topilmadi."
            };
        }

        if (storedCode != verificationCode)
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = "Tasdiqlash kodi noto'g'ri."
            };
        }

        // Redis'dan vaqtinchalik foydalanuvchi ma'lumotlarini olish
        var tempUserKey = $"temp_user:{email.ToLower()}";
        var tempUserData = await _redis.GetStringAsync(tempUserKey);

        if (tempUserData == null)
        {
            return new RequestResponseDto
            {
                Success = false,
                Message = "Foydalanuvchi ma'lumotlari topilmadi. Qayta ro'yxatdan o'ting."
            };
        }

        // Foydalanuvchi ma'lumotlarini `Users` jadvaliga qo'shish
        var tempUser = JsonConvert.DeserializeObject<User>(tempUserData);
        var user = new User
        {
            Email = tempUser.Email,
            Password = tempUser.Password,
            Role = tempUser.Role,
            BirthDate = tempUser.BirthDate,
            EmailConfirmAt = true
        };

        _context.Users.Add(user);
        await _unitOfWork.SaveChangesAsync();

        // Redis'dan vaqtinchalik ma'lumotlarni o'chirish
        await _redis.RemoveAsync(redisKey);       // Tasdiqlash kodi
        await _redis.RemoveAsync(tempUserKey);   // Vaqtinchalik foydalanuvchi

        return new RequestResponseDto
        {
            Success = true,
            Message = "Email muvaffaqiyatli tasdiqlandi. Foydalanuvchi tizimga qo'shildi."
        };
    }


    public string GenerateVerificationCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = chars[random.Next(chars.Length)];
        }

        return new string(code);
    }
    /*
    public async Task<RequestResponseDto> ResetPasswordRequest(string toEmail, string verificationCode)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == toEmail.ToLower());

            if (user == null)
            {
                return new RequestResponseDto
                {
                    Success = false,
                    Message = "Email tizimda topilmadi."
                };
            }

            var lastVerification = await _context.VerificationCodes
                .Where(vc => vc.Email.ToLower() == toEmail.ToLower() && vc.CodeType == CodeType.ResetPasswordCode)
                .OrderByDescending(vc => vc.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastVerification != null)
            {
                var reAttempt = (DateTime.UtcNow - lastVerification.CreatedAt).TotalSeconds;

                if (reAttempt < 60)
                {
                    return new RequestResponseDto
                    {
                        Success = false,
                        Message = $"Kodni qayta yuborish uchun {Math.Round(60 - reAttempt)} soniya kutishingiz kerak"
                    };
                }
            }

            // SMTP Clientni "using" bilan ochish
            using (var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = "Parolni tiklash kodini yuborish",
                    Body = EmailTemplate.GetVerificationEmailTemplate(user.Email, verificationCode),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }

            // Verification kodni saqlash
            var verificationRecord = new VerificationCode
            {
                Email = toEmail,
                Code = verificationCode,
                CodeType = CodeType.ResetPasswordCode,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                UserId = user.Id
            };

            _context.VerificationCodes.Add(verificationRecord);
            await _unitOfWork.SaveChangesAsync();

            return new RequestResponseDto
            {
                Success = true,
                Message = "Parolni tiklash xabari muvaffaqiyatli yuborildi!"
            };
        }
        catch (Exception ex)
        {
            // Xatolikni loglash yoki tekshirish uchun xatolikni batafsil olish
            return new RequestResponseDto
            {
                Success = false,
                Message = $"Parolni tiklash xabarini yuborishda xatolik: {ex.Message}"
            };
        }
    }
    */

    /*
    public async Task<RequestResponseDto> ResetMyPassword(string Email, string VerificationCode, string Password)
    {
        var verificationRecord = await _context.VerificationCodes
            .FirstOrDefaultAsync(vc => vc.Email.ToLower() == Email.ToLower() && vc.Code == VerificationCode && vc.CodeType == CodeType.ResetPasswordCode);

        if (verificationRecord != null && verificationRecord.ExpiresAt > DateTime.UtcNow)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == Email.ToLower());
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(new Users(), Password);
                _context.Users.Update(user);
                _context.VerificationCodes.Remove(verificationRecord);
                await _unitOfWork.SaveChangesAsync();

                return new RequestResponseDto
                {
                    Success = true,
                    Message = "Parol muvaffaqiyatli o'zgartirildi."
                };

            }
        }

        return new RequestResponseDto
        {
            Success = false,
            Message = "Parolni tiklashda xatolik."
        };
    }
    */
}
