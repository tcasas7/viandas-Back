namespace ViandasDelSur.Services.Implementations
{
    using System.Net;
    using System.Net.Mail;
    using ViandasDelSur.Services.Interfaces;

    public class EmailService : IEmailService
    {
        private readonly string fromEmail = "viandasdelsur37@gmail.com"; 
        private readonly string appPassword = "ckhomwlcavqsxetx";  

        public void SendResetPasswordEmail(string toEmail, string token)
        {
            var resetLink = $"https://5d4lf267-4200.brs.devtunnels.ms/reset-password?token={token}";


            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Restablecer tu contraseña",
                Body = $"Hola!\n\nHacé clic en este enlace para restablecer tu contraseña:\n{resetLink}\n\nEste enlace expira en 15 minutos.",
                IsBodyHtml = false
            };

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }

}
