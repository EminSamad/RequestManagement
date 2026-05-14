using ClosedXML.Excel;
using RequestManagement.Business.Interfaces;
using RequestManagement.Data.Repositories.Interfaces;

namespace RequestManagement.Business.BackgroundJobs;

public class ReportJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IReportService _reportService;

    public ReportJobService(IUnitOfWork unitOfWork, IEmailService emailService, IReportService reportService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _reportService = reportService;
    }

    public async Task SendWeeklyReport()
    {
        var report = await _reportService.GetReportAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Weekly Report");

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
        var excelBytes = stream.ToArray();

        // Get admins
        var users = await _unitOfWork.GetAllUsersWithRolesAsync();
        var admins = users
            .Where(u => u.UserRoles != null &&
                        u.UserRoles.Any(ur => ur.Role.Name == "Admin"))
            .ToList();

        foreach (var admin in admins)
        {
            await _emailService.SendEmailAsync(
                admin.Email,
                "Weekly Report",
                $"<h3>Weekly Report</h3><p>Please find the weekly report attached.</p>"
            );
        }
    }
}