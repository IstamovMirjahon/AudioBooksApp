using AudioBooks.Application.Interfaces.File;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudioBooks.Api.Controllers.File
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IJwtTokenService _jwtTokenService;
        public FileController(IFileService fileService, IJwtTokenService jwtTokenService)
        {
            _fileService = fileService;
            _jwtTokenService = jwtTokenService;
        }
        [HttpGet("GetAudio/{bookId}")]
        public async Task<IActionResult> GetAudio(Guid bookId)
        {
            try
            {
                var audioBytes = await _fileService.GetAudioFileAsync(bookId);

                var fileExtension = Path.GetExtension(bookId.ToString()).ToLower();
                string mimeType = fileExtension switch
                {
                    ".mp3" => "audio/mpeg",
                    ".wav" => "audio/wav",
                    ".ogg" => "audio/ogg",
                    _ => "application/octet-stream"
                };

                return File(audioBytes, mimeType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
        [HttpGet("GetDownload/{bookId}")]
        public async Task<IActionResult> GetDownload(Guid bookId)
        {
            try
            {
                var downloadBytes = await _fileService.GetDownloadFileAsync(bookId);

                var fileExtension = Path.GetExtension(bookId.ToString()).ToLower();
                string mimeType = fileExtension switch
                {
                    ".pdf" => "application/pdf",
                    ".epub" => "application/epub+zip",
                    ".mobi" => "application/x-mobipocket-ebook",
                    ".azw3" => "application/vnd.amazon.ebook",
                    ".txt" => "text/plain",
                    ".rtf" => "application/rtf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".html" => "text/html",
                    ".odt" => "application/vnd.oasis.opendocument.text",
                    _ => "application/octet-stream"
                };

                return File(downloadBytes, mimeType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpGet("GetImage/{bookId}")]
        public async Task<IActionResult> GetImage(Guid bookId)
        {
            try
            {
                var imageBytes = await _fileService.GetImageFileAsync(bookId);

                var fileExtension = Path.GetExtension(bookId.ToString()).ToLower();
                string mimeType = fileExtension switch
                {
                    ".jpg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return File(imageBytes, mimeType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }
    }
}
