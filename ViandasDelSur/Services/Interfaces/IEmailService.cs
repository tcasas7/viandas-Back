namespace ViandasDelSur.Services.Interfaces
{
    public interface IEmailService
    {
        void SendResetPasswordEmail(string toEmail, string token);
    }

}
