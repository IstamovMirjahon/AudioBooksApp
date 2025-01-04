using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Infrastructure.Repositories.BooksRepository;

public interface IBookRepository
{
    Task<RequestResponseDto<Book>> GetByIdAsync(Guid id);
    //Task<RequestResponseDto<BookResultDTO>> GetById(Guid id);
    Task<IEnumerable<BookResultDTO>> GetAllAsync();
    Task AddAsync(Book entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Book entity);
    Task UpdateAsync(Book entity);
}
