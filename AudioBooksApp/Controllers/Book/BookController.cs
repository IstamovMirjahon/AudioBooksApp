using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AudioBooksApp.Controllers.Book
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;
        private readonly IUserLibraryService _userLibraryService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ICommentService _commentService;
        public BookController(ICategoryService categoryService,
            IBookService bookService,
            ILogger<BookController> logger,
            IUserLibraryService userLibraryService,
            IJwtTokenService jwtTokenService,
            ICommentService commentService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _userLibraryService = userLibraryService;
            _commentService = commentService;
        }
        [HttpGet("Search Page")]
        [SwaggerOperation(
           Summary = "Search Book and Author",
           Description = "Qidirish author yoki book nomi orqali"
        )]
        public async Task<ActionResult<SearchPageDTO>> SearchBooknameOrAuthor(string? booknameOrAuthor)
        {
            var resultSearchValue = await _categoryService.SearchBookOrAuthor(booknameOrAuthor);

            return Ok(new SearchPageDTO { SearchQuery = booknameOrAuthor, Results = resultSearchValue.ToList() });
        }

        [HttpGet("GetAllBook")]
        [SwaggerOperation(
            Summary = "Get allbook",
            Description = "Hamma kitoblarni qaytaradi"
         )]
        public async Task<IActionResult> GetAllBook()
        {  
            var result = await _bookService.GetAllBook();
            return Ok(result.Value);
        }

        [HttpGet("GetBook")]
        [SwaggerOperation]
        public async Task<ActionResult> GetByIdBook(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var getByIdBook = await _bookService.GetBook(id);
            if (!getByIdBook.IsSuccess)
            {
                return NotFound("Kitob topilmadi.");
            }

            var userLibrary = new UserLibraryCreateDTO
            {
                UserId = userId.Value,
                BookId = id
            };

            var result = await _userLibraryService.CreateAsync(userLibrary);

            return Ok(getByIdBook.Value);
        }
        [HttpPost("Create")]
        [SwaggerOperation(
            Summary = "Create Book",
            Description = "Kitob qo'shish"
         )]
        public async Task<IActionResult> CreateBook(BookCreateDTO bookCreateDTO)
        {
            var result = await _bookService.CreateBookAsync(bookCreateDTO);
            return Ok("Muvaffaqiyatli qo'shildi");
        }
        [HttpPut("Update")]
        [SwaggerOperation(
            Summary = "Update Book",
            Description = "Bor kitobni yangilash"
         )]
        public async Task<IActionResult> UpdateBook(BookUpdateDTO bookUpdateDTO)
        {
            var result = await _bookService.UpdateBook(bookUpdateDTO);
            return Ok("Muvaffaqiyatli o'zgartirildi");
        }
        [HttpDelete("Delete")]
        [SwaggerOperation(
            Summary = "Delete Book",
            Description = "Kitob o'chirish"
         )]
        public async Task<IActionResult> DeleteBook(BookDeleteDTO bookDeleteDTO)
        {
            var result = await _bookService.DeleteBook(bookDeleteDTO);

            return Ok(result);
        }
        [HttpPost("{bookId}/comments")]
        public async Task<IActionResult> AddComment(Guid bookId, [FromBody] CommentCreateDTO commentDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var username = await _jwtTokenService.GetUserDetailsFromTokenAsync(token);

            var result = await _commentService.AddCommentAsync(bookId, commentDto, username.ToString());
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("{bookId}/comments")]
        public async Task<IActionResult> GetComments(Guid bookId)
        {
            var result = await _commentService.GetCommentsByBookIdAsync(bookId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
