using System.Net;
using System.Net.Mail;

namespace FaiscaMcQueen_Transportes.Services
{
    public class EmailService : IEmailService
    {
        // O IConfiguration serve para conseguirmos ler o appsettings.json
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailAsync(string emailDestino, string assunto, string mensagemHtml)
        {
            // 1. Lemos os dados do ficheiro de configurações
            var emailOrigem = _config["ConfiguracoesEmail:EmailOrigem"];
            var passwordApp = _config["ConfiguracoesEmail:PasswordApp"];
            var servidorSmtp = _config["ConfiguracoesEmail:ServidorSmtp"];
            var porta = int.Parse(_config["ConfiguracoesEmail:Porta"]);

            // 2. Preparamos a "carta" a ser enviada
            var carta = new MailMessage();
            carta.From = new MailAddress(emailOrigem, "TranspoTech");
            carta.To.Add(emailDestino);
            carta.Subject = assunto;
            carta.Body = mensagemHtml;
            carta.IsBodyHtml = true; // Permite-nos usar HTML (negritos, links) no email

            // 3. Ligamos a "mota" do estafeta (SmtpClient)
            using (var smtp = new SmtpClient(servidorSmtp, porta))
            {
                // Entregamos a nossa credencial (password de aplicação) ao Gmail
                smtp.Credentials = new NetworkCredential(emailOrigem, passwordApp);
                smtp.EnableSsl = true; // Obrigatório para o Gmail aceitar a ligação

                // 4. Enviamos a carta!
                await smtp.SendMailAsync(carta);
            }
        }
    }
}
