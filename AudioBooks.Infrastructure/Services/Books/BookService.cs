using AudioBooks.Application.Data;
using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Models.BookModels;
using AudioBooks.Infrastructure.Repositories.BooksRepository;

namespace AudioBooks.Infrastructure.Services.Books;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISqlConnection _sqlConnection;
    private readonly ICommentService _commentService;
    public BookService(IBookRepository bookRepository, IUnitOfWork unitOfWork, ISqlConnection sql, ICommentService commentService)
    {
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
        _sqlConnection = sql;
        _commentService = commentService;
    }
    public async Task<RequestResponseDto> CreateBookAsync(BookCreateDTO bookDto)
    {
        using var imageStream = new MemoryStream();
        using var downloadStream = new MemoryStream();
        using var audioStream = new MemoryStream();

        string imageFileName = null, downloadFileName = null, audioFileName = null;

        if (bookDto.ImageFile != null)
        {
            imageFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.ImageFile.FileName)}";
            await bookDto.ImageFile.CopyToAsync(imageStream);
        }

        if (bookDto.DownloadFile != null)
        {
            downloadFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.DownloadFile.FileName)}";
            await bookDto.DownloadFile.CopyToAsync(downloadStream);
        }

        if (bookDto.AudioFile != null)
        {
            audioFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.AudioFile.FileName)}";
            await bookDto.AudioFile.CopyToAsync(audioStream);
        }
        bookDto.SetReleaseDateUtc();

        var book = new Book
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Description = bookDto.Description,
            Price = bookDto.Price,
            ReleaseDate = bookDto.ReleaseDate,
            ImageFile = imageFileName,
            DownloadFile = downloadFileName,
            AudioFile = audioFileName,
            CreateDate = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow,
            IsDelete = false
        };
        if (bookDto.CategoryIds != null && bookDto.CategoryIds.Any())
        {
            book.BookCategories = bookDto.CategoryIds.Select(categoryId => new BookCategory
            {
                CategoryId = categoryId
            }).ToList();
        }

        await _bookRepository.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        if (!Directory.Exists(uploadDirectory))
            Directory.CreateDirectory(uploadDirectory);

        if (imageStream.Length > 0 && !string.IsNullOrEmpty(imageFileName))
        {
            var imagePath = Path.Combine(uploadDirectory, imageFileName);
            await File.WriteAllBytesAsync(imagePath, imageStream.ToArray());
        }

        if (downloadStream.Length > 0 && !string.IsNullOrEmpty(downloadFileName))
        {
            var downloadPath = Path.Combine(uploadDirectory, downloadFileName);
            await File.WriteAllBytesAsync(downloadPath, downloadStream.ToArray());
        }

        if (audioStream.Length > 0 && !string.IsNullOrEmpty(audioFileName))
        {
            var audioPath = Path.Combine(uploadDirectory, audioFileName);
            await File.WriteAllBytesAsync(audioPath, audioStream.ToArray());
        }

        return new RequestResponseDto { Message = $"Book created successfully. BookId = {book.Id}", Success = true };
    }


    public async Task<RequestResponseDto<Book>> DeleteBook(BookDeleteDTO book)
    {
        if (book == null)
        {
            return new RequestResponseDto<Book> { Message = "Book qiymati null", Success = false };
        }

        var chekId = await _bookRepository.GetByIdAsync(book.Id);

        if (chekId == null)
        {
            return new RequestResponseDto<Book> { Message = "Bookda bunaqa id yuq", Success = false };
        }

        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        DeleteFileIfExists(uploadDirectory, chekId.Data.ImageFile);
        DeleteFileIfExists(uploadDirectory, chekId.Data.DownloadFile);
        DeleteFileIfExists(uploadDirectory, chekId.Data.AudioFile);


        await _bookRepository.DeleteAsync(chekId.Data);
        await _unitOfWork.SaveChangesAsync();
        chekId.Data.DeleteDate = DateTime.UtcNow;
        chekId.Data.IsDelete = true;
        return new RequestResponseDto<Book> { Message = "Kitob uchirildi", Success = true, Data = chekId.Data };
    }

    public async Task<Result<IEnumerable<BookResultDTO>>> GetAllBook()
    {
        var result = await _bookRepository.GetAllAsync();

        return Result<IEnumerable<BookResultDTO>>.Success(result.ToList());
    }

    public async Task<Result<BookResultDTO>> GetBook(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Result<BookResultDTO>.Failure(new Error("Xatolik:", "Id null yoki noto‘g‘ri"));
        }

        var result = await _bookRepository.GetByIdAsync(id);

        if (!result.Success || result.Data == null)
        {
            return Result<BookResultDTO>.Failure(new Error("Xatolik:", "Kitob topilmadi"));
        }

        var book = result.Data;

        var bookResultDto = new BookResultDTO
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
            Rating = book.Rating, // Rating oldindan hisoblangan bo‘lsa
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
        };

        return Result<BookResultDTO>.Success(bookResultDto);
    }


    public async Task<RequestResponseDto> UpdateBook(BookUpdateDTO bookDto)
    {
        if (bookDto == null)
        {
            return new RequestResponseDto { Message = "Null qiymat", Success = false };
        }

        var existingBook = await _bookRepository.GetByIdAsync(bookDto.Id);
        if (existingBook == null)
        {
            return new RequestResponseDto { Message = "Book Not Found", Success = false };
        }

        bookDto.SetReleaseDateUtc();
        existingBook.Data.Title = bookDto.Title;
        existingBook.Data.Author = bookDto.Author;
        existingBook.Data.Description = bookDto.Description ?? existingBook.Data.Description;
        existingBook.Data.Price = bookDto.Price ?? existingBook.Data.Price;
        existingBook.Data.ReleaseDate = bookDto.ReleaseDate;
        existingBook.Data.UpdateDate = DateTime.UtcNow;

        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        if (!Directory.Exists(uploadDirectory))
        {
            Directory.CreateDirectory(uploadDirectory);
        }

        // Fayllarni o'chirish va yangilarini yuklash jarayoni
        if (bookDto.ImageUrl != null)
        {
            DeleteFileIfExists(uploadDirectory, existingBook.Data.ImageFile);

            var imageFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.ImageUrl.FileName)}";
            var imagePath = Path.Combine(uploadDirectory, imageFileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await bookDto.ImageUrl.CopyToAsync(stream);
            }
            existingBook.Data.ImageFile = imageFileName;
        }

        if (bookDto.DownloadUrl != null)
        {
            DeleteFileIfExists(uploadDirectory, existingBook.Data.DownloadFile);

            var downloadFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.DownloadUrl.FileName)}";
            var downloadPath = Path.Combine(uploadDirectory, downloadFileName);
            using (var stream = new FileStream(downloadPath, FileMode.Create))
            {
                await bookDto.DownloadUrl.CopyToAsync(stream);
            }
            existingBook.Data.DownloadFile = downloadFileName;
        }

        if (bookDto.AudioUrl != null)
        {
            DeleteFileIfExists(uploadDirectory, existingBook.Data.AudioFile);

            var audioFileName = $"{Guid.NewGuid()}{Path.GetExtension(bookDto.AudioUrl.FileName)}";
            var audioPath = Path.Combine(uploadDirectory, audioFileName);
            using (var stream = new FileStream(audioPath, FileMode.Create))
            {
                await bookDto.AudioUrl.CopyToAsync(stream);
            }
            existingBook.Data.AudioFile = audioFileName;
        }

        await _bookRepository.UpdateAsync(existingBook.Data);
        await _unitOfWork.SaveChangesAsync();

        return new RequestResponseDto { Message = "Successful", Success = true };
    }




    private void DeleteFileIfExists(string directory, string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            var filePath = Path.Combine(directory, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

}
