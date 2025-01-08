using AudioBooks.Application.Data;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.Models.BookModels;
using Dapper;

namespace AudioBooks.Infrastructure.Repositories.MyBooks;

public class UserLibraryRepository : IUserLibraryRepository
{
    private readonly ISqlConnection _sqlConnection;
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public UserLibraryRepository(ISqlConnection sqlConnection,
                                ApplicationDbContext applicationDbContext,
                                IUnitOfWork unitOfWork)
    {
        _context = applicationDbContext;
        _sqlConnection = sqlConnection;
        _unitOfWork = unitOfWork;
    }
    public void AddBookForLibraryAsync(UserLibrary userLibrary)
    {
        _context.userLibraries.Add(userLibrary);
        _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserLibrary>> GetAllLibraryBooksAsync(Guid userId)
    {
        using var connect = _sqlConnection.ConnectionBuild();

        const string sql = """
                               SELECT DISTINCT ON ("book_id") *
                               FROM "user_libraries"
                               WHERE "user_id" = @UserId
                               ORDER BY "book_id", "added_date" DESC
                           """;

        var query = await connect.QueryAsync<UserLibrary>(sql, new { UserId = userId });

        return query;
    }


    public async Task<UserLibrary> GetLibraryByIdAsync(Guid id)
    {
        var result = _context.userLibraries.FirstOrDefault(x => x.Id == id);

        return result;
    }

}
