using RequestManagement.Domain.DTOs.Report;
using RequestManagement.Domain.Interfaces;
using RequestManagement.Application.Interfaces;

namespace RequestManagement.Application.Services;

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
    public async Task<IEnumerable<ReportDto>> GetFilteredReportAsync(ReportFilterDto filter)
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();
        var categories = await _unitOfWork.Categories.GetAllAsync();

        var query = requests.AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchText))
            query = query.Where(r => r.Title.Contains(filter.SearchText) ||
                                     r.Description.Contains(filter.SearchText));

        if (filter.CategoryId.HasValue)
            query = query.Where(r => r.CategoryId == filter.CategoryId.Value);

        if (!string.IsNullOrEmpty(filter.Priority))
            query = query.Where(r => r.Priority.ToString() == filter.Priority);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(r => r.Status.ToString() == filter.Status);

        if (filter.StartDate.HasValue)
            query = query.Where(r => r.CreatedAt >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(r => r.CreatedAt <= filter.EndDate.Value);

        return query.Select(r => new ReportDto
        {
            RequestId = r.Id,
            Category = categories.FirstOrDefault(c => c.Id == r.CategoryId)!.Name ?? "Unknown",
            Priority = r.Priority.ToString(),
            Description = r.Description,
            CreatedBy = users.FirstOrDefault(u => u.Id == r.RequesterId)!.FullName ?? "Unknown",
            ResponseBy = r.ExecutorId.HasValue
                ? users.FirstOrDefault(u => u.Id == r.ExecutorId)!.FullName
                : null,
            ResponseTime = r.ModifiedAt,
            Status = r.Status.ToString()
        });
    }
}