using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure.Services.Books
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public UserPreferenceService(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponseDto> SaveUserPreferencesAsync(Guid id, UserPreferencesDTO preferences)
        {
            if (preferences.CategoryIds.Count < 3)
            {
                return new RequestResponseDto { Message = "kamida 3 ta category tanlang", Success = false };
            }

            var existingPreferences = await _context.UserPreferences
                .Where(up => up.UserId == id)
                .ToListAsync();
            
            _context.UserPreferences.RemoveRange(existingPreferences);

            var newPreferences = preferences.CategoryIds.Select(categoryId => new UserPreference
            {
                UserId = id,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.UserPreferences.AddRangeAsync(newPreferences);
            await _unitOfWork.SaveChangesAsync();

            return new RequestResponseDto { Message = "category saved", Success = true };
        }

        public async Task<Result<IEnumerable<BookResultDTO>>> GetRecommendedBooksAsync(Guid userId)
        {
            var recommendedBooks = await _context.Books
                .Where(b => !b.IsDelete)
                .Join(_context.BookCategories,
                    book => book.Id,
                    bc => bc.BookId,
                    (book, bc) => new { Book = book, CategoryId = bc.CategoryId })
                .Join(_context.UserPreferences.Where(up => up.UserId == userId),
                    b => b.CategoryId,
                    up => up.CategoryId,
                    (b, up) => new BookResultDTO
                    {
                        Id = b.Book.Id,
                        Title = b.Book.Title,
                        Author = b.Book.Author,
                        Description = b.Book.Description,
                        Download_file = b.Book.DownloadFile,
                        Audio_file = b.Book.AudioFile,
                        Image_file = b.Book.ImageFile,
                        Rating = b.Book.Rating, 
                    })
                .Distinct()
                .ToListAsync();

            return Result<IEnumerable<BookResultDTO>>.Success(recommendedBooks);
        }
    }
} 