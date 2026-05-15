using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RequestManagement.Business.Interfaces;

namespace RequestManagement.Business.Services;

public class EmailConsumerService : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceScopeFactory _scopeFactory;

    public EmailConsumerService(IRabbitMQService rabbitMQService, IServiceScopeFactory scopeFactory)
    {
        _rabbitMQService = rabbitMQService;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.ConsumeMessageAsync("email-queue", async message =>
        {
            var emailMessage = JsonSerializer.Deserialize<EmailMessage>(message);
            if (emailMessage == null) return;

            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            await emailService.SendEmailDirectAsync(
                emailMessage.To,
                emailMessage.Subject,
                emailMessage.Body);
        });

        return Task.CompletedTask;
    }
}

public class EmailMessage
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}