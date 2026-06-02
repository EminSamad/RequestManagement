using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.Enums;
using RequestManagement.Domain.Interfaces;

namespace RequestManagement.Application.BackgroundJobs;

public class ReminderJobService
{
    private readonly IUnitOfWork _unitOfWork;    
    private readonly IEmailService _emailService;

    public ReminderJobService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task SendReminder()
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();

        var overdueRequests = requests
            .Where(r => r.Status == RequestStatus.Initial 
                     && r.CreatedAt <= DateTime.UtcNow.AddHours(-24))
            .ToList();

        foreach (var request in overdueRequests)
        {
            var executors = users
                .Where(u => u.UserRoles != null && 
                            u.UserRoles.Any(ur => ur.Role.Name == "Executor"))
                .ToList();

            foreach (var executor in executors)
            {
                await _emailService.SendEmailAsync(
                    executor.Email,
                    "Reminder: Pending Request",
                    $"<h3>Reminder!</h3><p>Request '<b>{request.Title}</b>' has been pending for more than 24 hours. Please take action.</p>"
                );
            }
        }
    }
}