using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Interfaces.Token;
using AudioBooks.Domain.Models.UserModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //public async Task<string> GenerateToken(User user)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new Claim("UserId", user.Id.ToString()),
    //        new Claim("Role", user.Role.ToString())
    //    };

    //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:Key"]));
    //    var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    //    var tokenOptions = new JwtSecurityToken(
    //        issuer: _configuration["JwtOptions:Issuer"],
    //        audience: _configuration["JwtOptions:Audience"],
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtOptions:ExpiresInMinutes"])),
    //        signingCredentials: signingCredentials
    //    );

    //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

    //    var tokenKey = $"user_tokens:{user.Id}";
    //    await _cache.SetStringAsync(tokenKey, tokenString, new DistributedCacheEntryOptions
    //    {
    //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(300)
    //    });

    //    return tokenString;
    //}

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtOptions:Key"]);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtOptions:Issuer"],
            ValidAudience = _configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Result<Guid>> GetUserIdFromTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        // Token ichidan userId ni olish
        var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        return Result<Guid>.Success(Guid.Parse(userIdClaim.Value)); // userId ni qaytarish
        //try
        //{
        //    var handler = new JwtSecurityTokenHandler();

        //    if (!handler.CanReadToken(token)) return Result<Guid>.Failure(new Error ("Failed"));

        //    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        //    var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "UserId");
        //    if (userIdClaim == null) return Result<Guid>.Failure(new Error("Failed:", "null value"));

        //    Guid userId = default;
        //    if (Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
        //    {
        //        userId = parsedUserId;
        //    }

        //    return Result<Guid>.Success(userId);
        //}
        //catch
        //{
        //    return Result<Guid>.Failure(new Error("Failed"));
        //}
    }

    public async Task<Result<string>> GetUserRoleFromTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token)) return Result<string>.Failure(new Error("Failed"));

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var roleClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "Role");
            if (roleClaim == null) return Result<string>.Failure(new Error("Failed ","null value"));

            string role = roleClaim.Value;
            return Result<string>.Success(role);
        }
        catch
        {
            return Result<string>.Failure(new Error("Failed"));
        }
    }

    public async Task<Result<(Guid UserId, string Username)>> GetUserDetailsFromTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return Result<(Guid, string)>.Failure(new Error("Failed", "Invalid token"));

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            // Token ichidan UserId ni olish
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                return Result<(Guid, string)>.Failure(new Error("Failed", "UserId not found in token"));

            // Token ichidan Username ni olish
            var usernameClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.UniqueName);
            if (usernameClaim == null)
                return Result<(Guid, string)>.Failure(new Error("Failed", "Username not found in token"));

            // UserId ni Guid formatiga o'zgartirish
            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                return Result<(Guid, string)>.Failure(new Error("Failed", "Invalid UserId format"));

            var username = usernameClaim.Value;

            return Result<(Guid, string)>.Success((userId, username));
        }
        catch (Exception ex)
        {
            return Result<(Guid, string)>.Failure(new Error("Failed", ex.Message));
        }
    }

}


/*
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public JwtTokenService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<(string, string)> GenerateTokenAsync(Users user)
    {
        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Role", user.Role.ToString()),
            new Claim("IpAddress", _httpContextAccessor.HttpContext.GetClientIpAddress())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtOptions:Key").Value!));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration.GetSection("JwtOptions:Issuer").Value!,
            audience: _configuration.GetSection("JwtOptions:Audience").Value!,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration.GetSection("JwtOptions:ExpiresInMinutes").Value!)),
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var access_token = tokenHandler.WriteToken(tokenOptions);

        var refreshToken = GenerateRefreshToken(user.Id);

        // Save or update token in database
        var existingToken = await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == user.Id);
        if (existingToken != null)
        {
            existingToken.JwtToken = access_token;
            existingToken.RefreshToken = refreshToken;
            existingToken.JwtTokenExpiryTime = tokenOptions.ValidTo;
            existingToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1);
            _context.Tokens.Update(existingToken);
        }
        else
        {
            await _context.Tokens.AddAsync(new Tokens
            {
                UserId = user.Id,
                JwtToken = access_token,
                RefreshToken = refreshToken,
                JwtTokenExpiryTime = tokenOptions.ValidTo,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(1)
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return (access_token, refreshToken);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSettings = _configuration.GetSection("JwtOptions");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            // Token muddati tugagan
            Console.WriteLine("Token muddati tugagan");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            // Token imzosi noto'g'ri
            Console.WriteLine("Token imzosi noto'g'ri");
            return null;
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            // Issuer noto'g'ri
            Console.WriteLine("Issuer noto'g'ri");
            return null;
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            // Audience noto'g'ri
            Console.WriteLine("Audience noto'g'ri");
            return null;
        }
        catch (Exception ex)
        {
            // Boshqa xatoliklar
            Console.WriteLine($"Tokenni tekshirishda xatolik: {ex.Message}");
            return null;
        }
    }

    public async Task<(string, string)> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = ValidateToken(token);
        if (principal == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        var storedToken = await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == userId && t.RefreshToken == refreshToken);

        if (storedToken == null || storedToken.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new SecurityTokenException("User not found");
        }

        return await GenerateTokenAsync(user);
    }

    private string GenerateRefreshToken(Guid userId)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtOptions:Key").Value!));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Fix for CS1503
            new Claim("IssuedAt", DateTime.UtcNow.ToString()), // Fix for CS1503
            new Claim("ExpiresAt", DateTime.UtcNow.AddMonths(1).ToString()) // Fix for CS1503
        };

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration.GetSection("JwtOptions:Issuer").Value!,
            audience: _configuration.GetSection("JwtOptions:Audience").Value!,
            claims: claims,
            expires: DateTime.UtcNow.AddMonths(1),
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.WriteToken(tokenOptions);

        return jwtToken;
    }

    public async Task<Result<Guid>> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token)) return Result.Failure<Guid>(Error.None);


            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "UserId");
            if (userIdClaim == null) return Result.Failure<Guid>(Error.NullValue);

            Guid userId = default;
            if (Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
            {
                userId = parsedUserId;
            }

            return Result.Success<Guid>(userId);
        }
        catch
        {
            return Result.Failure<Guid>(Error.NullValue);
        }
    }
    public async Task<Result<string>> GetUserRoleFromTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token)) return Result.Failure<string>(Error.None);

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var roleClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "Role");
            if (roleClaim == null) return Result.Failure<string>(Error.NullValue);

            string role = roleClaim.Value;
            return Result.Success<string>(role);
        }
        catch
        {
            return Result.Failure<string>(Error.NullValue);
        }
    }

}
*/