using RequestManagement.Core.DTOs.Report;

namespace RequestManagement.Business.Interfaces;

public interface IReportService
{
    Task<IEnumerable<ReportDto>> GetReportAsync();
    Task<IEnumerable<ReportDto>> GetFilteredReportAsync(ReportFilterDto filter);
}