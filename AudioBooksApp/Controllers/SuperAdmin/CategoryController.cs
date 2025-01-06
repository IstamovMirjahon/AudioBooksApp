using AudioBooks.Application.Interfaces.Books;
using AudioBooks.Domain.DTOs;
using AudioBooks.Domain.DTOs.Book;
using AudioBooks.Domain.Interfaces.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AudioBooks.Api.Controllers.SuperAdmin
{
    [ApiController]
    [Route("api/v1/superadmin/[controller]/[action]")]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IJwtTokenService _jwtTokenService;

        public CategoryController(ICategoryService service, IJwtTokenService jwtTokenService)
        {
            _categoryService = service;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("GetAllCategory")]
        [SwaggerOperation(
             Summary = "Hamma categorylarni olish"
         )]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResultDTO>>> GetAllCategory()
        {
            var category = await _categoryService.GetAllCategory();

            return Ok(category.Value);
        }

        [HttpGet("SearchCategories")]
        [SwaggerOperation(
            Summary = "Search Category",
            Description = "Category qidirish"
        )]
        [AllowAnonymous]
        public async Task<ActionResult> SearchCategories(string? value)
        {
            var result = await _categoryService.SearchCategory(value);
            return Ok(result);
        }
        [HttpPost("Create")]
        [SwaggerOperation(
            Summary = "CategoryAdd",
            Description = "Category turlarini qo'shish"
        )]
        public async Task<ActionResult<RequestResponseDto>> CreateCategory(CategoryCreateDTO createDTO)
        {
            if (createDTO is null)
            {
                return new RequestResponseDto { Message = "Malumotlarni to'ldiring", Success = false };
            }
            var category = await _categoryService.CreateCategory(createDTO);

            return new RequestResponseDto { Message = "Category qo'shildi", Success = true };
        }
        [HttpPut("Update")]
        [SwaggerOperation(
           Summary = "CategoryChange",
           Description = "Category turini o'zgartirish"
        )]
        public async Task<IActionResult> UpdateCategory(CategoryUpdateDTO updateDTO)
        {
            if (updateDTO is null)
            {
                return BadRequest("Qiymatlarni to'ldiring");
            }
            var resultupdate = await _categoryService.UpdateCategory(updateDTO);

            return Ok(resultupdate.Success);
        }

        [HttpDelete("Delete")]
        [SwaggerOperation(
           Summary = "CategoryRemove",
           Description = "Category o'zgartirish"
        )]
        public async Task<IActionResult> DeleteCategory(CategoryDeleteDTO deleteDTO)
        {
            if (deleteDTO is null)
            {
                return NotFound("To'g'ri ma'lumot kiriting");
            }
            var delete = await _categoryService.DeleteCategory(deleteDTO);

            return Ok(delete.Success);
        }
    }
}
