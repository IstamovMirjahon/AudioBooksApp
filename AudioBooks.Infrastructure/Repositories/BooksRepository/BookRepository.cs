using AudioBooks.Application.Data;
using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AudioBooks.Infrastructure.Repositories.BooksRepository;

public class BookRepository : Repository<Book>, IBookRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ISqlConnection _sqlConnection;
    private readonly ICommentService _commentService;

    public BookRepository(ApplicationDbContext applicationDbContext, ISqlConnection sql, ICommentService commentService) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
        _sqlConnection = sql;
        _commentService = commentService;
    }

    public async Task<IEnumerable<BookResultDTO>> GetAllAsyncDapper()
    {
        using var connect = _sqlConnection.ConnectionBuild();

        const string sql = """
                        SELECT 
                            b."id", b."title", b."author", b."description", b."image_file", b."price", b."release_date", b."download_file", b."audio_file", b."rating",
                            c."id" AS CategoryId, c."name" AS CategoryName, c."description" AS CategoryDescription,
                            cm."id" AS CommentId, cm."user_name" AS CommentUserName, cm."text" AS CommentText, cm."created_at" AS CommentCreatedAt
                        FROM "books" b
                        LEFT JOIN "book_categories" bc ON b."id" = bc."book_id"
                        LEFT JOIN "categories" c ON bc."category_id" = c."id"
                        LEFT JOIN "comments" cm ON b."id" = cm."book_id"
                        """;

        var bookDictionary = new Dictionary<Guid, BookResultDTO>();

        var query = await connect.QueryAsync<BookResultDTO, CategoryResultDTO, CommentResultDTO, BookResultDTO>(
            sql,
            (book, category, comment) =>
            {
                if (!bookDictionary.TryGetValue(book.Id, out var bookEntry))
                {
                    bookEntry = book;
                    bookEntry.Categories = new List<CategoryResultDTO>();
                    bookEntry.Comments = new List<CommentResultDTO>();
                    bookDictionary.Add(bookEntry.Id, bookEntry);
                }

                if (category != null && category.Id != Guid.Empty && !bookEntry.Categories.Any(c => c.Id == category.Id))
                {
                    bookEntry.Categories.Add(category);
                }

                if (comment != null && comment.Id != Guid.Empty && !bookEntry.Comments.Any(c => c.Id == comment.Id))
                {
                    bookEntry.Comments.Add(comment);
                }

                return bookEntry;
            },
            splitOn: "CategoryId,CommentId"
        );

        return bookDictionary.Values;
    }
    public async Task<IEnumerable<BookResultDTO>> GetAllAsync()
    {
        var books = await _applicationDbContext.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Comments)
            .ToListAsync();

        var bookRatings = await _applicationDbContext.Comments
            .GroupBy(c => c.BookId)
            .Select(g => new
            {
                BookId = g.Key,
                AverageRating = g.Average(c => c.Value)
            })
            .ToDictionaryAsync(x => x.BookId, x => x.AverageRating);

        var bookResultDtos = books.Select(book => new BookResultDTO
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            Download_file = book.DownloadFile,
            Audio_file = book.AudioFile,
            Image_file = book.ImageFile,
            Description = book.Description,
            release_date = book.ReleaseDate,
            Rating = bookRatings.ContainsKey(book.Id) ? bookRatings[book.Id] : 0, 
            Categories = book.BookCategories.Select(bc => new CategoryResultDTO
            {
                Id = bc.Category.Id,
                Name = bc.Category.Name,
                Description = bc.Category.Description
            }).ToList(),
            Comments = book.Comments.Select(c => new CommentResultDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Text = c.Text,
                Value = c.Value,
                CreatedAt = c.CreatedAt
            }).ToList()
        });

        return bookResultDtos;
    }


    public async Task<RequestResponseDto<BookResultDTO>> GetByIdAsyncDapper(Guid id)
    {
        using var connect = _sqlConnection.ConnectionBuild();

        const string sql = """
                        SELECT 
                            b."id", b."title", b."author", b."description", b."image_file", b."price", b."release_date", b."download_file", b."audio_file", b."rating",
                            c."id" AS CategoryId, c."name" AS CategoryName, c."description" AS CategoryDescription,
                            cm."id" AS CommentId, cm."user_name" AS CommentUserName, cm."text" AS CommentText, cm."created_at" AS CommentCreatedAt
                        FROM "books" b
                        LEFT JOIN "book_categories" bc ON b."id" = bc."book_id"
                        LEFT JOIN "categories" c ON bc."category_id" = c."id"
                        LEFT JOIN "comments" cm ON b."id" = cm."book_id"
                        WHERE b."id" = @Id
                        """;

        var bookDictionary = new Dictionary<Guid, BookResultDTO>();

        var query = await connect.QueryAsync<BookResultDTO, CategoryResultDTO, CommentResultDTO, BookResultDTO>(
            sql,
            (book, category, comment) =>
            {
                if (!bookDictionary.TryGetValue(book.Id, out var bookEntry))
                {
                    bookEntry = book;
                    bookEntry.Categories = new List<CategoryResultDTO>();
                    bookEntry.Comments = new List<CommentResultDTO>();
                    bookDictionary.Add(bookEntry.Id, bookEntry);
                }

                if (category != null && category.Id != Guid.Empty && !bookEntry.Categories.Any(c => c.Id == category.Id))
                {
                    bookEntry.Categories.Add(category);
                }

                if (comment != null && comment.Id != Guid.Empty && !bookEntry.Comments.Any(c => c.Id == comment.Id))
                {
                    bookEntry.Comments.Add(comment);
                }

                return bookEntry;
            },
            new { Id = id },
            splitOn: "CategoryId,CommentId"
        );

        var bookResult = bookDictionary.Values.FirstOrDefault();

        if (bookResult == null)
        {
            return new RequestResponseDto<BookResultDTO>
            {
                Success = false,
                Message = "Book not found"
            };
        }

        return new RequestResponseDto<BookResultDTO>
        {
            Success = true,
            Data = bookResult
        };
    }
    public async Task<RequestResponseDto<Book>> GetByIdAsync(Guid id)
    {
        var book = await _applicationDbContext.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return new RequestResponseDto<Book>
            {
                Success = false,
                Message = "Book not found"
            };
        }

        var rating = await _commentService.CalculateAverageCommentValueAsync(id);
        book.Rating = rating.Value;
        return new RequestResponseDto<Book>
        {
            Success = true,
            Data = book
        };
    }
}
