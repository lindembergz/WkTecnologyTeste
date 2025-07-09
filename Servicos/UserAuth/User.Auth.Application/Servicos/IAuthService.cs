using UserAuth.Aplicacao.DTOs;

namespace UserAuth.Aplicacao.Servicos
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<TokenDto?> LoginAsync(LoginDto loginDto);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<TokenDto?> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<TokenDto?> VerifyTwoFactorCodeAsync(string username, string code);
        Task<bool> EnableTwoFactorAuthAsync(string username);
        Task<bool> DisableTwoFactorAuthAsync(string username);
    }
}
