using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAuth.Aplicacao.DTOs;
using UserAuth.Aplicacao.Servicos;

namespace WkTecnology.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]

    [IgnoreAntiforgeryToken]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }
            var profile = await _userService.GetProfileAsync(username);
            if (profile == null)
            {
                return NotFound();
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            var username = User.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }
            var result = await _userService.UpdateProfileAsync(username, updateUserProfileDto);
            if (!result)
            {
                return BadRequest("Update failed.");
            }
            return Ok("Update successful.");
        }
    }
}
