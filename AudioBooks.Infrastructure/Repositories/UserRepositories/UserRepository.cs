using AudioBooks.Domain.Models.UserModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using AudioBooks.Application.Data;
using AudioBooks.Domain.StaticModels;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure.Repositories.UserRepositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ISqlConnection _sqlConnection;
    private readonly SmtpSettings _smtpSettings;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConfiguration _configuration;
    public UserRepository(ApplicationDbContext context, IOptions<SmtpSettings> smtpSettings,
        ApplicationDbContext applicationDbContext,
        ISqlConnection sqlConnection, IConfiguration configuration) : base(context)
    {
        _smtpSettings = smtpSettings.Value;
        _applicationDbContext = applicationDbContext;
        _sqlConnection = sqlConnection;
        _configuration = configuration;
    }

    public async Task<bool> CheckConfirmPassword(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }

    public async Task<bool> VerificationCode(int inputCode, int sendCode, DateTime createCodeTime)
    {
        return inputCode == sendCode && DateTime.Now < createCodeTime.AddMinutes(1);
    }

    public async Task<int> GenerateCode()
    {
        return new Random().Next(1000, 10000);
    }

    public async Task SendWelcomeEmail(string email, string fullName)
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
            Body = EmailTemplate.GetWelcomeEmailTemplate(email, fullName),
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);
        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendVerificationEmail(string email, int code)
    {
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
                Subject = "Tasdiqlash kodi",
                Body = EmailTemplate.GetVerificationEmailTemplate(code),
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
        string sql = """
                        SELECT 
                            id,
                            full_name AS FullName,
                            user_name AS UserName,
                            phone_number AS PhoneNumber,
                            birth_date AS BirthDate,
                            email,
                            email_confirm_at AS EmailConfirmAt,
                            password,
                            role,
                            balance,
                            status,
                            create_date AS CreateDate,
                            update_date AS UpdateDate,
                            delete_date AS DeleteDate,
                            is_delete AS IsDelete
                        FROM "users" 
                    """;

        using var connect = _sqlConnection.ConnectionBuild();
        var users = await connect.QueryAsync<User>(sql);
        return users.ToList();
    }


    public async Task<User> GetByEmailAsync(string email)
    {
        User? user = await _applicationDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
        return user;
    }
    public async Task<string> GenerateToken(string userId, string username, string role)
    {
        // Konfiguratsiya sozlamalarini olish
        var secretKey = _configuration["JwtSettings:Secret"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expiryMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var result) ? result : 60; // Default to 60 minutes

        // Tokenni shifrlash uchun kalit
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Token ichidagi foydalanuvchi ma'lumotlari (Claims)
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),  // User ID
        new Claim(JwtRegisteredClaimNames.UniqueName, username),    // Username
        new Claim(ClaimTypes.Role, role),                           // Role
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique Token ID
    };

        // Tokenni yaratish
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GetUserIdFromToken(string token)
    {
        // Tokenni dekodlash
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Token ichidan userId ni olish
        var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        return userIdClaim.Value; // userId ni qaytarish
    }


}
