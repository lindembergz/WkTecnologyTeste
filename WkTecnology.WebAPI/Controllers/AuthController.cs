using Microsoft.AspNetCore.Mvc;
using UserAuth.Aplicacao.DTOs;
using UserAuth.Aplicacao.Servicos;


namespace WkTecnology.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [IgnoreAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result)
            {
                return BadRequest("Registration failed.");
            }
            return Ok("Registration successful. Please confirm your email.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var tokenDto = await _authService.LoginAsync(loginDto);
            if (tokenDto == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            if (tokenDto.TwoFactorRequired)
            {
                return Ok(tokenDto); // Retorna que 2FA é necessário
            }

            return Ok(tokenDto);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(email, token);
            if (!result)
            {
                return BadRequest("Email confirmation failed.");
            }
            return Ok("Email confirmed successfully.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            if (tokenDto.AccessToken == null || tokenDto.RefreshToken == null)
            {
                return BadRequest("Invalid token data.");
            }
            var newTokens = await _authService.RefreshTokenAsync(tokenDto.AccessToken, tokenDto.RefreshToken);
            if (newTokens == null)
            {
                return Unauthorized("Invalid tokens.");
            }
            return Ok(newTokens);
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyTwoFactorDto verifyDto)
        {
            var tokenDto = await _authService.VerifyTwoFactorCodeAsync(verifyDto.Username, verifyDto.Code);
            if (tokenDto == null)
            {
                return Unauthorized("Invalid 2FA code.");
            }
            return Ok(tokenDto);
        }

        // DTO para VerifyTwoFactorCode
        public class VerifyTwoFactorDto
        {
            public required string Username { get; set; }
            public required string Code { get; set; }
        }
    }
}
