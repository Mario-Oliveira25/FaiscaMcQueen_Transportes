namespace FaiscaMcQueen_Transportes.Services
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string emailDestino, string assunto, string mensagemFormatoHtml);
    }
}
