using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Report;

namespace RequestManagement.API.Endpoints.Report;

public class GetFilteredReportEndpoint : Endpoint<ReportFilterDto>
{
    private readonly IReportService _reportService;

    public GetFilteredReportEndpoint(IReportService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("/api/report/filter");
        Roles("Admin");
    }

    public override async Task HandleAsync(ReportFilterDto req, CancellationToken ct)
    {
        var report = await _reportService.GetFilteredReportAsync(req);
        await HttpContext.Response.WriteAsJsonAsync(report, ct);
    }
}