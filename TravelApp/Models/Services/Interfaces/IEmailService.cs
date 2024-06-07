namespace TravelApp.Models.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string To, string Subject, string Body);
    }
}
