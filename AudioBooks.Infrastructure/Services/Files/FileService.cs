using AudioBooks.Application.Interfaces.File;
using AudioBooks.Infrastructure.Repositories.BooksRepository;
using Microsoft.Extensions.Configuration;

namespace AudioBooks.Infrastructure.Services.Files;

public class FileService : IFileService
{
    private readonly string _audioFilesDirectory;
    private readonly IBookRepository _bookRepository;

    public FileService(IConfiguration configuration, IBookRepository bookRepository)
    {
        _audioFilesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        _bookRepository = bookRepository;
    }

    public async Task<byte[]> GetAudioFileAsync(Guid bookId)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null || string.IsNullOrEmpty(book.Data.AudioFile))
            {
                throw new FileNotFoundException("Audio file not found for this book.");
            }

            var audioFilePath = Path.Combine(_audioFilesDirectory, book.Data.AudioFile);

            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException($"Audio file '{audioFilePath}' does not exist.");
            }

            var audioBytes = await File.ReadAllBytesAsync(audioFilePath);

            return audioBytes;
        }
        catch (Exception ex)
        {
            throw new FileNotFoundException("Error occurred while retrieving audio file.", ex);
        }
    }

    public async Task<byte[]> GetDownloadFileAsync(Guid bookId)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null || string.IsNullOrEmpty(book.Data.DownloadFile))
            {
                throw new FileNotFoundException("Download file not found for this book");
            }

            var downloadFilePath = Path.Combine(_audioFilesDirectory, book.Data.DownloadFile);

            if (!File.Exists(downloadFilePath))
            {
                throw new FileNotFoundException($"Download file {downloadFilePath} does not exist");
            }

            var downloadBytes = await File.ReadAllBytesAsync(downloadFilePath);

            return downloadBytes;

        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"{ex.Message}", ex);
        }
    }

    public async Task<byte[]> GetImageFileAsync(Guid bookId)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null || string.IsNullOrEmpty(book.Data.DownloadFile))
            {
                throw new FileNotFoundException("Download file not found for this book");
            }

            var imageFilePath = Path.Combine(_audioFilesDirectory, book.Data.ImageFile);

            if (!File.Exists(imageFilePath))
            {
                throw new FileNotFoundException($"Download file {imageFilePath} does not exist");
            }

            var imageBytes = await File.ReadAllBytesAsync(imageFilePath);

            return imageBytes;

        }
        catch (Exception ex)
        {
            throw new FileNotFoundException($"{ex.Message}", ex);
        }
    }
}
