using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;

public interface IUserPreferenceService
{
    Task<RequestResponseDto> SaveUserPreferencesAsync(Guid id, UserPreferencesDTO preferences);
    Task<Result<IEnumerable<BookResultDTO>>> GetRecommendedBooksAsync(Guid userId); 
} 