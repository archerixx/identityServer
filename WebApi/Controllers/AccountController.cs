using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Domain.Interfaces.Identity;
using Project.Domain.Models.Identity;
using Project.WebApi.Models;

namespace Project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await userService.GetByIdAsync(id.ToString());
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userRequest)
        {
            var userResult = await userService.CreateAsync(userRequest.User, userRequest.password);
            if (!userResult.Succeeded)
                return BadRequest(userResult.Errors);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(Users user)
        {
            var userResult = await userService.UpdateAsync(user);
            if (!userResult.Succeeded)
                return BadRequest(userResult.Errors);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(Users user)
        {
            var userResult = await userService.DeleteAsync(user);
            if (!userResult.Succeeded)
                return BadRequest(userResult.Errors);
            return Ok();
        }
    }
}
