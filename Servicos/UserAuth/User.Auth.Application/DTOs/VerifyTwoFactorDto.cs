namespace UserAuth.Aplicacao.DTOs
{
    public class VerifyTwoFactorDto
    {
        public string? Username { get; set; }
        public string? Code { get; set; }
    }
}
