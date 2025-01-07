using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Mvc;

namespace AudioBooksApp.Controllers.User
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserCommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IJwtTokenService _jwtTokenService;

        public UserCommentController(ICommentService commentService, IJwtTokenService jwtTokenService)
        {
            _commentService = commentService;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Kitobga yangi sharh qo'shadi.
        /// </summary>
        /// <param name="bookId">Kitob ID-si</param>
        /// <param name="commentDto">Sharh ma'lumotlari</param>
        /// <param name="username">Foydalanuvchi nomi</param>
        /// <returns>Sharh qo'shilganligi haqida natija</returns>
        [HttpPost("{bookId}/add-comment")]
        public async Task<IActionResult> AddCommentAsync(Guid bookId, [FromBody] CommentCreateDTO commentDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var username = await _jwtTokenService.GetUserDetailsFromTokenAsync(token);
            if (commentDto == null || string.IsNullOrWhiteSpace(username.Value.Username))
            {
                return BadRequest("Comment data or username is missing.");
            }

            var result = await _commentService.AddCommentAsync(bookId, commentDto, username.Value.Username);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error.Message);
        }

        /// <summary>
        /// Berilgan kitobga tegishli barcha sharhlarni oladi.
        /// </summary>
        /// <param name="bookId">Kitob ID-si</param>
        /// <returns>Kitob sharhlari</returns>
        [HttpGet("{bookId}/comments")]
        public async Task<IActionResult> GetCommentsByBookIdAsync(Guid bookId)
        {
            var result = await _commentService.GetCommentsByBookIdAsync(bookId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return NotFound(result.Error.Message);
        }
    }
}
