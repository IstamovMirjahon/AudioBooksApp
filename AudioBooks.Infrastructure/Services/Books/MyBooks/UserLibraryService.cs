using AudioBooks.Application.Data;
using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using AudioBooks.Infrastructure.Repositories.MyBooks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace AudioBooks.Infrastructure.Services.Books.MyBooks;

public class UserLibraryService : IUserLibraryService
{
    private readonly IUserLibraryRepository _userLibraryRepository;
    private readonly ISqlConnection _sqlconnection;
    private readonly ILogger<UserLibraryService> _logger;
    public UserLibraryService(IUserLibraryRepository userLibraryRepository, ILogger<UserLibraryService> logger, ISqlConnection sqlConnection)
    {
        _userLibraryRepository = userLibraryRepository;
        _logger = logger;
        _sqlconnection = sqlConnection;
    }
    public async Task<RequestResponseDto> CreateAsync(UserLibraryCreateDTO userLibraryCreateDTO)
    {
        try
        {
            if (userLibraryCreateDTO == null)
            {
                return new RequestResponseDto { Message = "qiymat bush", Success = false };
            }

            var userlibrary = new UserLibrary()
            {
                User_Id = userLibraryCreateDTO.UserId,
                Book_Id = userLibraryCreateDTO.BookId,
                AddedDate = DateTime.UtcNow,
            };
            _userLibraryRepository.AddBookForLibraryAsync(userlibrary);
            return new RequestResponseDto { Message = "Muvaffaqiyatli qo'shildi", Success = true }; ;
        }
        catch (Exception ex)
        {
            _logger.LogError("Create jarayonida xatolik " + ex.Message);
            return new RequestResponseDto { Message = "xatolik sodir bo'ldi", Success = false };
        }
    }

    public async Task<Result<IEnumerable<UserLibrary>>> GetAllAsync()
    {
        try
        {
            var result = await _userLibraryRepository.GetAllLibraryBooksAsync();
            return Result<IEnumerable<UserLibrary>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}");
            return Result<IEnumerable<UserLibrary>>.Failure(new Error ("",""));
        }
    }

    public async Task<Result<UserLibraryDTO>> GetLibraryBook(Guid id)
    {
        if (id.ToString() is null)
        {
            return Result<UserLibraryDTO>.Failure(new Error ("Xatolik: ", "id null"));
        }

        var result = await _userLibraryRepository.GetLibraryByIdAsync(id);

        if (result == null)
        {
            return Result<UserLibraryDTO>.Failure(new Error("Xatolik : ", "Not Found"));
        }

        var resultDto = new UserLibraryDTO
        {
            BookId = result.Book_Id,
            UserId = result.User_Id,
            AddedDate = result.AddedDate
        };

        return Result<UserLibraryDTO>.Success(resultDto);
    }

    public async Task<RequestResponseDto<IEnumerable<UserLibrary>>> SearchBook(string value)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                return new RequestResponseDto<IEnumerable<UserLibrary>> { Message = "value null", Success = false };
            }

            var query = """
                           SELECT ul.*
                           FROM "user_libraries" ul
                           JOIN "books" b ON ul."book_id" = b."id"
                           WHERE LOWER(b."title") LIKE LOWER(@SearchTerm) OR LOWER(b."author") LIKE LOWER(@SearchTerm)
                        """;


            using var connection = _sqlconnection.ConnectionBuild();

            var books = await connection.QueryAsync<UserLibrary>(
                query, new { SearchTerm = "%" + value.ToLower() + "%" });

            return new RequestResponseDto<IEnumerable<UserLibrary>> { Message = "Succesful", Success = true, Data = books.ToList() };
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}", ex);
            return new RequestResponseDto<IEnumerable<UserLibrary>> { Message = "Failed", Success = false };
        }
    }
}
