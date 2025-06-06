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
                    (b, up) => b.Book)
                .Distinct()
                .Select(book => new BookResultDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Price = book.Price,
                    Description = book.Description,
                    Download_file = book.DownloadFile,
                    Audio_file = book.AudioFile,
                    Image_file = book.ImageFile,
                    Rating = book.Rating,
                    Categories = _context.BookCategories
                        .Where(bc => bc.BookId == book.Id)
                        .Join(_context.Categories,
                            bc => bc.CategoryId,
                            c => c.Id,
                            (bc, c) => new CategoryResultDTO
                            {
                                Id = c.Id,
                                Name = c.Name,
                                Description = c.Description
                            })
                        .ToList()
                })
                .ToListAsync();

            return Result<IEnumerable<BookResultDTO>>.Success(recommendedBooks);
        }

    }
} 