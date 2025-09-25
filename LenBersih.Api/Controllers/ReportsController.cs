using Microsoft.AspNetCore.Mvc;
using LenBersih.Shared;

namespace LenBersih.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private static readonly List<Report> _reports = new();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Report>>> GetReports()
    {
        return Ok(_reports);
    }

    [HttpPost]
    public async Task<ActionResult<Report>> CreateReport(Report report)
    {
        report.Id = _reports.Count + 1;
        report.DateReported = DateTime.UtcNow;
        _reports.Add(report);
        return CreatedAtAction(nameof(GetReports), new { id = report.Id }, report);
    }
}