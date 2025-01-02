namespace AudioBooks.Application.Interfaces.File;

public interface IFileService
{
    Task<byte[]> GetAudioFileAsync(Guid bookId);
    Task<byte[]> GetDownloadFileAsync(Guid bookId);
    Task<byte[]> GetImageFileAsync(Guid bookId);
}
