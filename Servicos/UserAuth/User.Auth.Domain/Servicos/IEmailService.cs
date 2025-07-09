namespace UserAuth.Dominio.Servicos
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string token);
        Task SendTwoFactorCodeAsync(string email, string code);
    }
}
