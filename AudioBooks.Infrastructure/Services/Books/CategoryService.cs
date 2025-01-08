using AudioBooks.Application.Data;
using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using AudioBooks.Infrastructure.Repositories.CategorysBooksRepository;
using Microsoft.EntityFrameworkCore;

namespace AudioBooks.Infrastructure.Services.Books;

public class CategoryService : ICategoryService
{
    private readonly ISqlConnection _sqlConnection;
    private readonly ICategoryBooksRepository _categoryBooksRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CategoryService(ISqlConnection sqlConnection, ICategoryBooksRepository category, IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _sqlConnection = sqlConnection;
        _categoryBooksRepository = category;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<RequestResponseDto> CreateCategory(CategoryCreateDTO category)
    {
        if (category == null)
        {
            return new RequestResponseDto { Message = "null value", Success = false};
        }
        var categorynew = new Category()
        {
            Name = category.Name,
            Description = category.Description,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
        };
        var result = _categoryBooksRepository.AddAsync(categorynew);
        await _unitOfWork.SaveChangesAsync();

        return new RequestResponseDto { Message = "Succesfull", Success = true };

    }

    public async Task<RequestResponseDto> DeleteCategory(CategoryDeleteDTO category)
    {
        var checkId = await _categoryBooksRepository.GetByIdAsync(category.Id);

        if (checkId == null)
        {
            return new RequestResponseDto { Message = "null value", Success = false };
        }

        await _categoryBooksRepository.DeleteAsync(checkId);
        await _unitOfWork.SaveChangesAsync();


        return new RequestResponseDto { Message = "category deleted", Success = true};
    }

    public async Task<Result<IEnumerable<CategoryResultDTO>>> GetAllCategory()
    {
        var result = await _categoryBooksRepository.GetAllCategories();
        return Result<IEnumerable<CategoryResultDTO>>.Success(result);
    }

    public async Task<IEnumerable<BookResultDTO>> SearchBookOrAuthor(string? value)
    {
        var query = _context.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Comments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(value))
        {
            query = query.Where(b => b.Title.ToLower().Contains(value.ToLower()) || b.Author.ToLower().Contains(value.ToLower()));
        }

        var books = await query.ToListAsync();

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
            Rating = book.Rating,
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

    public async Task<IEnumerable<CategoryResultDTO>> SearchCategory(string? value)
    {
        var result = await _categoryBooksRepository.SearchCategory(value);

        return result;
    }

    public async Task<RequestResponseDto> UpdateCategory(CategoryUpdateDTO category)
    {
        var existingCategory = await _categoryBooksRepository.GetByIdAsync(category.Id);

        if (existingCategory == null)
        {
            return new RequestResponseDto { Message = "null value", Success = false };
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;
        existingCategory.UpdateDate = DateTime.UtcNow;

        await _categoryBooksRepository.UpdateAsync(existingCategory);
        await _unitOfWork.SaveChangesAsync();

        return new RequestResponseDto { Message = "Succesfull", Success = true };
    }

}
