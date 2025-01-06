using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Application.Interfaces.Books;

public interface IUserLibraryService
{
    Task<RequestResponseDto> CreateAsync(UserLibraryCreateDTO userLibraryCreateDTO);
    Task<Result<IEnumerable<UserLibrary>>> GetAllAsync(Guid userId);
    Task<Result<UserLibraryDTO>> GetLibraryBook(Guid id);
    Task<RequestResponseDto<IEnumerable<UserLibrary>>> SearchBook(string value); 
}
