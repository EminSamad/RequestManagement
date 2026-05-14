using RequestManagement.Business.Interfaces;

namespace RequestManagement.Business.BackgroundJobs;

public class EmailJobService
{
    private readonly IEmailService _emailService;

    public EmailJobService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        await _emailService.SendEmailAsync(to, subject, body);
    }
}