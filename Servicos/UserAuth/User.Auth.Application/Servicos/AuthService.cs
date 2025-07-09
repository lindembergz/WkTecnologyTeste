using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserAuth.Aplicacao.DTOs;
using UserAuth.Dominio.Entidades;
using UserAuth.Dominio.Repositories;
using UserAuth.Dominio.Servicos;
using UserAuth.Dominio.UoW;

namespace UserAuth.Aplicacao.Servicos
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            IUserUnitOfWork unitOfWork,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return false; // Usuário já existe
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User(registerDto.Username, registerDto.Email, passwordHash);

            // Gerar token de confirmação de e-mail
            user.SetEmailConfirmationToken(Guid.NewGuid().ToString());

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync();

            // Enviar e-mail de confirmação
            //await _emailService.SendEmailConfirmationAsync(user.Email, user.EmailConfirmationToken);

            return true;
        }

        public async Task<TokenDto?> LoginAsync(LoginDto loginDto)
        {
            if (loginDto.Username == null || loginDto.Password == null)
            {
                return null;
            }

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null; // Credenciais inválidas
            }

            if (!user.IsEmailConfirmed)
            {
                return new TokenDto { TwoFactorRequired = false }; // Indica que e-mail precisa ser confirmado
            }

            // if (user.IsTwoFactorEnabled)
            // {
            //     // Gerar e enviar código 2FA
            //     user.SetTwoFactorCode(GenerateTwoFactorCode(), DateTime.UtcNow.AddMinutes(5));
            //     _userRepository.Update(user);
            //     await _unitOfWork.CommitAsync();
            //     await _emailService.SendTwoFactorCodeAsync(user.Email, user.TwoFactorCode);

            //     return new TokenDto { TwoFactorRequired = true };
            // }

            return await GenerateTokens(user);
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.EmailConfirmationToken != token)
            {
                return false; // Usuário ou token inválido
            }

            user.ConfirmEmail();
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<TokenDto?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal?.Identity?.Name;

            if (username == null)
            {
                return null;
            }

            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null; // Refresh token inválido ou expirado
            }

            return await GenerateTokens(user);
        }

        public async Task<TokenDto?> VerifyTwoFactorCodeAsync(string username, string code)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.TwoFactorCode != code || user.TwoFactorCodeExpiryTime <= DateTime.UtcNow)
            {
                return null; // Código 2FA inválido ou expirado
            }

            user.ClearTwoFactorCode();
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return await GenerateTokens(user);
        }

        public async Task<bool> EnableTwoFactorAuthAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            user.EnableTwoFactorAuth();
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DisableTwoFactorAuthAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            user.DisableTwoFactorAuth();
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return true;
        }

        private async Task<TokenDto> GenerateTokens(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var accessTokenExpirationMinutes = _configuration["Jwt:AccessTokenExpirationMinutes"];
            var refreshTokenExpirationDays = _configuration["Jwt:RefreshTokenExpirationDays"];

            if (key == null || issuer == null || audience == null || accessTokenExpirationMinutes == null || refreshTokenExpirationDays == null)
            {
                throw new InvalidOperationException("JWT settings are not configured.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(accessTokenExpirationMinutes));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Guid.NewGuid().ToString();

            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(Convert.ToDouble(refreshTokenExpirationDays)));
            _userRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TwoFactorRequired = user.IsTwoFactorEnabled && !user.TwoFactorCodeExpiryTime.HasValue // Se 2FA ativado e código não enviado ainda
            };
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (key == null || issuer == null || audience == null)
            {
                throw new InvalidOperationException("JWT settings are not configured.");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateLifetime = false // Não valida o tempo de vida do token expirado
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string GenerateTwoFactorCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }
}
