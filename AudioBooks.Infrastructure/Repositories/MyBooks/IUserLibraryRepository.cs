using AudioBooks.Domain.Models.BookModels;

namespace AudioBooks.Infrastructure.Repositories.MyBooks;

public interface IUserLibraryRepository
{
    void AddBookForLibraryAsync(UserLibrary userLibrary);
    Task<IEnumerable<UserLibrary>> GetAllLibraryBooksAsync();
    Task<UserLibrary> GetLibraryByIdAsync(Guid id);
}
