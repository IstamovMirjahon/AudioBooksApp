using AudioBooks.Application.Service.UserServices;
using AudioBooks.Domain.Interfaces.Token;
using AudioBooks.Domain.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AudioBooks.Api.Controllers.SuperAdmin
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IJwtTokenService _jwtTokenService;
        public UserController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            this.userService = userService;
            _jwtTokenService = jwtTokenService;
        }
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            var result = await userService.GetAllUsers();
            return Ok(result.Value);
        }
        [HttpGet("GetUserById")] 
        public async Task<ActionResult> GetUserById()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var result = await userService.GetUserAsync(userId.Value);
            return Ok(result.Value);
        }
        //[HttpPost("CreateUser")]
        //[AllowAnonymous]
        //public async Task<ActionResult> CreateUser(UserCreateDto userCreateDto)
        //{
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var role = await _jwtTokenService.GetUserRoleFromTokenAsync(token);
        //    var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            
        //    var result = await userService.CreateUserAsync(userCreateDto);
        //    return Ok(result.Data);
        //}
        [HttpPut("UpdateUser")]
        public async Task<ActionResult> UpdateUser (UserUpdateDto userUpdateDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var result = await userService.UpdateUserAsync(userUpdateDto,userId.Value);
            return Ok(result.Value);
        }
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

            var result = await userService.DeleteUserAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("UserChangePassword")]
        public async Task<ActionResult> UserChangePassword(ChangePassword passwordChangeDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var role = await _jwtTokenService.GetUserRoleFromTokenAsync(token);
            var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);


            var result = await userService.ChangePasswordAsync(passwordChangeDto, userId.Value);
            return Ok(result);
        }
        //[HttpPost("AdminChangePassword")]
        //[AllowAnonymous]
        //public async Task<ActionResult> AdminChangePassword(AdminPasswordChangeDto passwordChangeDto)
        //{
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var role = await _jwtTokenService.GetUserRoleFromTokenAsync(token);
        //    var userId = await _jwtTokenService.GetUserIdFromTokenAsync(token);

        //    if (!role.IsSuccess)
        //    {
        //        return Unauthorized();
        //    }
        //    if (!(await _jwtTokenService.CheckToken(userId.Value)))
        //    {
        //        return Unauthorized();
        //    }
        //    if (role.Value == "User")
        //    {
        //        return Unauthorized();
        //    }
        //    var result = await userService.AdminChangePasswordAsync(userId.Value, passwordChangeDto);
        //    return Ok(result);
        //}

        //[HttpPost("LogoutUser")]
        //[AllowAnonymous]
        //public async Task<ActionResult> LogoutUser(Guid id)
        //{
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var role = await _jwtTokenService.GetUserRoleFromTokenAsync(token);
        //    if (!role.IsSuccess)
        //    {
        //        return Unauthorized();
        //    }
           
        //    var result = await userService.LogoutUserAsync(id);
        //    return Ok(result);
        //}

    }
}
