using Microsoft.AspNetCore.Mvc;
using LenBersih.Shared;

namespace LenBersih.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private static readonly List<Report> _reports = new();

    [HttpGet]
    public ActionResult<IEnumerable<Report>> GetReports()
    {
        return Ok(_reports);
    }

    [HttpPost]
    public ActionResult<Report> CreateReport(Report report)
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

        report.CaptchaInput = string.Empty;

        report.Id = _reports.Count + 1;
        report.DateReported = DateTime.UtcNow;
        _reports.Add(report);
        return CreatedAtAction(nameof(GetReports), new { id = report.Id }, report);
    }
}