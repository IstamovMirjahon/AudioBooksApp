using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Infrastructure.Repositories.CategorysBooksRepository;

public interface ICategoryBooksRepository
{
    Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryResultDTO>> SearchCategory(string value);
    Task<IEnumerable<CategoryResultDTO>> GetAllCategories();
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
    
}
