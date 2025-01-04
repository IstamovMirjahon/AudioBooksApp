using AudioBooks.Application.Data;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using Dapper;

namespace AudioBooks.Infrastructure.Repositories.CategorysBooksRepository;

public class CategoryBooksRepository : Repository<Category>, ICategoryBooksRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ISqlConnection _sqlConnection;

    public CategoryBooksRepository(ApplicationDbContext context, ISqlConnection sql) : base(context)
    {
        _context = context;
        _sqlConnection = sql;
    }

    public Task<IEnumerable<Category>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CategoryResultDTO>> GetAllCategories()
    {
        using var connect = _sqlConnection.ConnectionBuild();

        const string sql = """
                            SELECT 
                                "id", "name", "description" 
                            FROM "categories"
                            """;

        var query = await connect.QueryAsync<CategoryResultDTO>(sql);

        return query;
    }

    public async Task<IEnumerable<CategoryResultDTO>> SearchCategory(string? value)
    {
        using var connect = _sqlConnection.ConnectionBuild();

        string sql;

        if (string.IsNullOrEmpty(value))
        {
            sql = """
                    SELECT *
                    FROM "categories"
                  """;
    
        var query = await connect.QueryAsync<CategoryResultDTO>(sql);
            return query;
        }
        else
        {
            sql = """
                    SELECT * 
                    FROM "categories"
                    WHERE LOWER("name") LIKE @Value
                """;
    
        var query = await connect.QueryAsync<CategoryResultDTO>(sql, new { Value = $"%{value.ToLower()}%" });
            return query;
        }
    }

}
