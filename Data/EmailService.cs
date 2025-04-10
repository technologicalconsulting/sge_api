using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _smtpServer = "smtp.mailtrap.io";
    private readonly int _smtpPort = 2525;
    private readonly string _smtpUser = "TU_USUARIO_MAILTRAP";
    private readonly string _smtpPass = "TU_PASSWORD_MAILTRAP";

    public async Task SendVerificationEmail(string destinatario, string codigo)
    {
        try
        {
            string htmlBody = File.ReadAllText("../src/CodigoVerificacion.html")
                                  .Replace("[CÓDIGO]", codigo);

            using var smtp = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress("no-reply@technologicalconsulting.com", "Technological Consulting"),
                Subject = "Código de Verificación",
                Body = htmlBody,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            await smtp.SendMailAsync(mensaje);
            Console.WriteLine("Correo enviado correctamente (Mailtrap).");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al enviar el correo: " + ex.Message);
        }
    }
}
