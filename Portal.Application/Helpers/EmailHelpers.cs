using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Helpers
{
    public static class EmailHelpers
    {
        public static async Task SendPasswordResetEmailAsync(string userEmail, string newPassword)
        {
            try
            {
                var smtpServer = "smtp.yourserver.com";
                var smtpPort = 587;
                var smtpUsername = "yourusername";
                var smtpPassword = "yourpassword";

                var fromAddress = new MailAddress("noreply@yourdomain.com", "E-Commerce Application");
                var toAddress = new MailAddress(userEmail);
                var subject = "Recuperação de Senha";
                var body = $"Sua nova senha é: {newPassword}";

                using (var smtp = new SmtpClient(smtpServer, smtpPort))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    })
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enviar e-mail de recuperação de senha.", ex);
            }
        }
    }
   
}
