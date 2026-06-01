namespace RequestManagement.Application.Interfaces;
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailDirectAsync(string to, string subject, string body);
}