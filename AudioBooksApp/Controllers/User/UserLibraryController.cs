using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AudioBooks.Api.Controllers.User
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserLibraryController : ControllerBase
    {
        private readonly IUserLibraryService _userLibraryService;
        private readonly IJwtTokenService _jwtTokenService;
        public UserLibraryController(IUserLibraryService userLibrary, IJwtTokenService jwtTokenService)
        {
            _userLibraryService = userLibrary;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("GetAllUserLibraryBooks")]
        public async Task<ActionResult> GetAllUserLibrary()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);


            var result = await _userLibraryService.GetAllAsync(userId.Value);

            return Ok(result.Value);
        }

        [HttpGet("GetUserLibraryById")]
        public async Task<ActionResult> GetUserLibraryBookById(Guid id)
        {
            var result = await _userLibraryService.GetLibraryBook(id);
            return Ok(result.Value);
        }

        [HttpGet("SearchUserLibrary")]
        public async Task<ActionResult> SearchUserLibrary(string value)
        {
            var result = await _userLibraryService.SearchBook(value);

            return Ok(result.Data);
        }
    }
}
