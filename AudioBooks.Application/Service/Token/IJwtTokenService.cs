using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Models.UserModels;
using System.Security.Claims;

namespace AudioBooks.Domain.Interfaces.Token;

public interface IJwtTokenService
{
    /*
    Task<(string, string)> GenerateTokenAsync(Users user);
    Task<Result<Guid>> GetUserIdFromTokenAsync(string token);
    Task<Result<string>> GetUserRoleFromTokenAsync(string token);   
    //Task<string> GenerateToken(string userId, string username, string role);
    ClaimsPrincipal ValidateToken(string token);
    Task<(string,string)> RefreshTokenAsync(string token, string refreshToken);
    */

    //Task<string> GenerateToken(User user);
    ClaimsPrincipal ValidateToken(string token);
    Task<Result<Guid>> GetUserIdFromTokenAsync(string token);
    Task<Result<string>> GetUserRoleFromTokenAsync(string token);
    //Task<bool> CheckToken(Guid userId);
    Task<Result<(Guid UserId, string Username)>> GetUserDetailsFromTokenAsync(string token);

}
