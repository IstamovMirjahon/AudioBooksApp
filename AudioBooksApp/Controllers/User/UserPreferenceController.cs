using AudioBooks.Domain.Abstractions;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AudioBooks.Api.Controllers.User
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserPreferenceController : ControllerBase
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly IJwtTokenService _jwtTokenService;

        public UserPreferenceController(IUserPreferenceService userPreferenceService, IJwtTokenService jwtTokenService)
        {
            _userPreferenceService = userPreferenceService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("SavePreferences")]
        [SwaggerOperation(
            Summary = "Save user category preferences",
            Description = "Save at least 3 preferred categories for personalized recommendations"
        )]
        public async Task<ActionResult> SavePreferences(UserPreferencesDTO preferences)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var result = await _userPreferenceService.SaveUserPreferencesAsync(userId.Value, preferences);

            return Ok(result);
        }

        [HttpGet("GetRecommendations")]
        [SwaggerOperation(
            Summary = "Get recommended books",
            Description = "Get book recommendations based on user's preferred categories"
        )]
        public async Task<ActionResult<Result<IEnumerable<BookResultDTO>>>> GetRecommendations()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var result = await _userPreferenceService.GetRecommendedBooksAsync(userId.Value);
            return Ok(result.Value);
        }
    }
}