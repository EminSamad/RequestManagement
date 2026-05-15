using System.Text.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RequestManagement.Business.Interfaces;
 
namespace RequestManagement.Business.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly IRabbitMQService _rabbitMQService;

    public EmailService(IConfiguration configuration, IRabbitMQService rabbitMQService)
    {
        _configuration = configuration;
        _rabbitMQService = rabbitMQService;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Publish to RabbitMQ queue
        var message = JsonSerializer.Serialize(new
        {
            To = to,
            Subject = subject,
            Body = body
        });

        _rabbitMQService.PublishMessageAsync("email-queue", message);

        await Task.CompletedTask;
    }

    public async Task SendEmailDirectAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["MailSettings:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["MailSettings:Host"],
            int.Parse(_configuration["MailSettings:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _configuration["MailSettings:Username"],
            _configuration["MailSettings:Password"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}