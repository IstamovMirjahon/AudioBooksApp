using AudioBooks.Application.Service.UserServices;
using AudioBooks.Domain.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AudioBooksApp.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [Authorize]
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Yaxshi");
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Userning tizimga kirishi",
        Description = "Userning tizimga kirishi uchun email va parolingizni kiriting va bosing"
        )]
    public async Task<IActionResult> SignIn(UserLoginDto signInDTO)
    {
        try
        {
            var result = await _userService.SignInAsync(signInDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "User registratsiya qilish",
        Description = "User registratsiya qilish uchun ma'lumotlarni to'ldiring va bosing"
        )]
    public async Task<IActionResult> SignUp(UserRegisterDto userDTO)
    {
        try
        {
            var result = await _userService.SignUpService(userDTO);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    [SwaggerOperation(
           Summary = "Kodni tasdiqlash",
           Description = "Emailingizga yuborilgan kodni kiriting va bosing"
           )]
    public async Task<IActionResult> Verification(int code)
    {
        try
        {
            var result = await _userService.VerificationCodeService(code);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [SwaggerOperation(
           Summary = "Parolni restart qilish",
           Description = "Ro'yhatdan o'tgan emailingiz orqali parolingizni restart qiling"
           )]
    public async Task<IActionResult> UserRessetPassword(RessetPasswordUserDTO userRessetPassword)
    {
        try
        {
            var result = await _userService.RessetPasswordAsync(userRessetPassword);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
