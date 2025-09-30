using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using LenBersih.Shared;
using DNTCaptcha.Core;
using LenBersih.Api.Services;

namespace LenBersih.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowBlazorWasm")]
public class ReportsController : ControllerBase
{
    private static readonly List<Report> _reports = new();
    private readonly IEmailService _emailService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IEmailService emailService, ILogger<ReportsController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Report>> GetReports()
    {
        return Ok(_reports);
    }

    [HttpPost]
    public async Task<ActionResult<Report>> CreateReport(Report report)
    {
        if (report is null)
        {
            return BadRequest("Report payload is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (report.IsAnonymous && string.IsNullOrWhiteSpace(report.ReporterName))
        {
            report.ReporterName = "Anonim";
        }

        if (!report.IsAnonymous && string.Equals(report.ReporterName, "Anonim", StringComparison.OrdinalIgnoreCase))
        {
            report.ReporterName = string.Empty;
        }

        if (report.EvidenceData is not null)
        {
            if (!ReportMetadata.IsAllowedEvidenceContentType(report.EvidenceContentType))
            {
                return BadRequest("Format file bukti tidak didukung.");
            }

            if (!ReportMetadata.IsEvidenceSizeValid(report.EvidenceData.LongLength))
            {
                return BadRequest($"Ukuran file bukti melebihi {ReportMetadata.MaxEvidenceSize / (1024 * 1024)} MB.");
            }
        }

        report.Id = _reports.Count + 1;
        report.DateReported = DateTime.UtcNow;
        _reports.Add(report);

        // Send email notification
        try
        {
            await _emailService.SendReportNotificationAsync(report);
            _logger.LogInformation("Email notification sent successfully for Report ID: {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification for Report ID: {ReportId}", report.Id);
            // Continue without failing the request - the report is still saved
        }

        return CreatedAtAction(nameof(GetReports), new { id = report.Id }, report);
    }
}