using RequestManagement.Core.DTOs.Report;
using RequestManagement.Data.Repositories.Interfaces;
using RequestManagement.Business.Interfaces;

namespace RequestManagement.Business.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ReportDto>> GetReportAsync()
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var categories = await _unitOfWork.Categories.GetAllAsync();

        return requests.Select(r => new ReportDto
        {
            RequestId = r.Id,
            Category = categories.FirstOrDefault(c => c.Id == r.CategoryId)?.Name ?? "Unknown",
            Priority = r.Priority.ToString(),
            Description = r.Description,
            CreatedBy = users.FirstOrDefault(u => u.Id == r.RequesterId)?.FullName ?? "Unknown",
            ResponseBy = r.ExecutorId.HasValue
                ? users.FirstOrDefault(u => u.Id == r.ExecutorId)?.FullName
                : null,
            ResponseTime = r.ModifiedAt,
            Status = r.Status.ToString()
        });
    }
}