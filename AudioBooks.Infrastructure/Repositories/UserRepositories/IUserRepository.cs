using AudioBooks.Domain.Models.UserModels;

namespace AudioBooks.Infrastructure.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<bool> CheckConfirmPassword(string password, string confirmPassword);
        Task<bool> VerificationCode(int inputCode, int sendCode, DateTime createCodeTime);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task<int> GenerateCode();
        Task SendWelcomeEmail(string email, string fullName);
        Task SendVerificationEmail(string email, int code);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetUsersAsync();
        Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<string> GenerateToken(string userId, string username, string role);
        Task<string> GetUserIdFromToken(string token);
    }
}
