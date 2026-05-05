using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Business.Interfaces;

namespace RequestManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet]
    //TODO add filter
    public async Task<IActionResult> GetReport()
    {
        _logger.LogInformation("Admin fetching report");
        var report = await _reportService.GetReportAsync();
        _logger.LogInformation("Report fetched successfully with {Count} records", report.Count());
        return Ok(report);
    }

//tod
    [HttpGet("export-excel")]
    //TODO filter
    public async Task<IActionResult> ExportToExcel()
    {
        _logger.LogInformation("Admin exporting report to Excel");
        var report = await _reportService.GetReportAsync();

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

        _logger.LogInformation("Report exported to Excel successfully");
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "report.xlsx");
    }
}