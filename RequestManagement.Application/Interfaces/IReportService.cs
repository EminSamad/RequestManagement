using RequestManagement.Core.DTOs.Report;

namespace RequestManagement.Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetReportAsync();
    Task<IEnumerable<ReportDto>> GetFilteredReportAsync(ReportFilterDto filter);
}