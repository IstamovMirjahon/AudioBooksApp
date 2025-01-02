using AudioBooks.Domain.DTOs;

namespace AudioBooks.Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<RequestResponseDto> SendEmailAsync(string toEmail, string subject, string message);
        Task<RequestResponseDto> SendWelcomeEmailAsync(string toEmail, string password);
        Task<RequestResponseDto> SendVerificationEmailAsync(string toEmail, string verificationCode);
        Task<RequestResponseDto> VerifyEmailAsync(string email, string verificationCode);
        Task<RequestResponseDto> ResetPasswordRequest(string ToEmail, string verificationCode);
        Task<RequestResponseDto> ResetMyPassword(string Email, string VerificationCode, string Password);
        string GenerateVerificationCode(int length = 6);
    }
}
