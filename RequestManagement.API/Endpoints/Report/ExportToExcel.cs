using ClosedXML.Excel;
using FastEndpoints;
using RequestManagement.Application.Interfaces;
using RequestManagement.Domain.DTOs.Report;

namespace RequestManagement.API.Endpoints.Report;

public class ExportToExcelEndpoint : Endpoint<ReportFilterDto>
{
    private readonly IReportService _reportService;

    public ExportToExcelEndpoint(IReportService reportService)
    {
        _reportService = reportService;
    }

    public override void Configure()
    {
        Get("/api/report/export-excel");
        Roles("Admin");
    }

    public override async Task HandleAsync(ReportFilterDto req, CancellationToken ct)
    {
        var report = await _reportService.GetFilteredReportAsync(req);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        worksheet.Cell(1, 1).Value = "Request ID";
        worksheet.Cell(1, 2).Value = "Category";
        worksheet.Cell(1, 3).Value = "Priority";
        worksheet.Cell(1, 4).Value = "Description";
        worksheet.Cell(1, 5).Value = "Created By";
        worksheet.Cell(1, 6).Value = "Response By";
        worksheet.Cell(1, 7).Value = "Response Time";
        worksheet.Cell(1, 8).Value = "Status";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;

        int row = 2;
        foreach (var item in report)
        {
            worksheet.Cell(row, 1).Value = item.RequestId;
            worksheet.Cell(row, 2).Value = item.Category;
            worksheet.Cell(row, 3).Value = item.Priority;
            worksheet.Cell(row, 4).Value = item.Description;
            worksheet.Cell(row, 5).Value = item.CreatedBy;
            worksheet.Cell(row, 6).Value = item.ResponseBy ?? "-";
            worksheet.Cell(row, 7).Value = item.ResponseTime?.ToString("yyyy-MM-dd HH:mm") ?? "-";
            worksheet.Cell(row, 8).Value = item.Status;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Response.Headers.Append("Content-Disposition", "attachment; filename=report.xlsx");
        await HttpContext.Response.Body.WriteAsync(stream.ToArray(), ct);
    }
}