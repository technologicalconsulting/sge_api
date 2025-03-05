using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace sge_api.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 📌 Método para enviar correos electrónicos
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPass"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true; // Office 365 requiere SSL

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
                return false;
            }
        }

        // 📌 Enviar Código de Verificación
        public async Task<bool> SendVerificationEmail(string toEmail, string verificationCode)
        {
            string subject = "Código de Verificación - Registro";
            string body = $@"
                <h2>Bienvenido a SGE</h2>
                <p>Tu código de verificación para completar el registro es:</p>
                <h3 style='color:blue;'>{verificationCode}</h3>
                <p>Este código expira en 15 minutos.</p>";

            return await SendEmailAsync(toEmail, subject, body);
        }

        // 📌 Enviar Credenciales de Acceso
        public async Task<bool> SendUserCredentials(string toEmail, string username, string password)
        {
            string subject = "Tus Credenciales de Acceso - SGE";
            string body = $@"
                <h2>Acceso a SGE</h2>
                <p>Tu cuenta ha sido creada con éxito.</p>
                <p><b>Usuario:</b> {username}</p>
                <p><b>Contraseña:</b> {password}</p>
                <p>Te recomendamos cambiar tu contraseña después de iniciar sesión.</p>";

            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}
