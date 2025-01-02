using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;

namespace AudioBooks.Application.Interfaces.Books;

public interface ICategoryService
{
    Task<IEnumerable<BookResultDTO>> SearchBookOrAuthor(string value);
    Task<IEnumerable<CategoryResultDTO>> SearchCategory(string value);
    Task<Result<IEnumerable<CategoryResultDTO>>> GetAllCategory();
    Task<RequestResponseDto> CreateCategory(CategoryCreateDTO category);
    Task<RequestResponseDto> UpdateCategory(CategoryUpdateDTO category);
    Task<RequestResponseDto> DeleteCategory(CategoryDeleteDTO category);

}
