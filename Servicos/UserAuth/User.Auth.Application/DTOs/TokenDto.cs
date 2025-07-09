namespace UserAuth.Aplicacao.DTOs
{
    public class TokenDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool TwoFactorRequired { get; set; }
    }
}
