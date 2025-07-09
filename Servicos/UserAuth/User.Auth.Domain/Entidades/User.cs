using WkTecnology.Core;

namespace UserAuth.Dominio.Entidades
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsEmailConfirmed { get; private set; }
        public string? EmailConfirmationToken { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }
        public string? TwoFactorCode { get; private set; }
        public DateTime? TwoFactorCodeExpiryTime { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        // Construtor para EF Core
        private User() 
        {
            Username = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public User(string username, string email, string passwordHash)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            IsEmailConfirmed = false;
            IsTwoFactorEnabled = false;
        }

        public void ConfirmEmail()
        {
            IsEmailConfirmed = true;
            EmailConfirmationToken = null;
        }

        public void SetEmailConfirmationToken(string token)
        {
            EmailConfirmationToken = token;
        }

        public void EnableTwoFactorAuth()
        {
            IsTwoFactorEnabled = true;
        }

        public void DisableTwoFactorAuth()
        {
            IsTwoFactorEnabled = false;
            TwoFactorCode = null;
            TwoFactorCodeExpiryTime = null;
        }

        public void SetTwoFactorCode(string code, DateTime expiryTime)
        {
            TwoFactorCode = code;
            TwoFactorCodeExpiryTime = expiryTime;
        }

        public void ClearTwoFactorCode()
        {
            TwoFactorCode = null;
            TwoFactorCodeExpiryTime = null;
        }

        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        public void UpdateProfile(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }
}
