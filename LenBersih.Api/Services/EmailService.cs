using MailKit.Net.Smtp;
using MimeKit;
using LenBersih.Shared;

namespace LenBersih.Api.Services;

public interface IEmailService
{
    Task SendReportNotificationAsync(Report report);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendReportNotificationAsync(Report report)
    {
        try
        {
            var message = new MimeMessage();

            // Sender (from your Gmail account or configured SMTP)
            message.From.Add(new MailboxAddress("Len Bersih System", _configuration["EmailSettings:FromEmail"]));

            // Recipient
            message.To.Add(new MailboxAddress("Report Administrator", "raihanrafliansyahdaring@gmail.com"));

            // Subject
            message.Subject = $"New Whistleblowing Report - {report.Category} (ID: {report.Id})";

            // Email body content
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GenerateEmailBody(report);
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Connect to SMTP server
            var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
            await client.ConnectAsync(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls);

            // Authenticate
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            // Send email
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Report notification email sent successfully for Report ID: {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification for Report ID: {ReportId}", report.Id);
            throw;
        }
    }

    private static string GenerateEmailBody(Report report)
    {
        var anonymousInfo = report.IsAnonymous ? "Anonim" : report.ReporterName;
        var evidenceInfo = !string.IsNullOrEmpty(report.EvidenceFileName)
            ? $"<p><strong>Bukti:</strong> {report.EvidenceFileName} ({report.EvidenceContentType})</p>"
            : "<p><strong>Bukti:</strong> Tidak ada file yang dilampirkan</p>";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc2626; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; border: 1px solid #ddd; }}
        .section {{ margin-bottom: 20px; }}
        .label {{ font-weight: bold; color: #555; }}
        .value {{ margin-left: 10px; }}
        .footer {{ background: #f5f5f5; padding: 15px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚨 New Whistleblowing Report</h1>
            <p>Len Bersih - E-Whistleblowing System</p>
        </div>
        
        <div class='content'>
            <div class='section'>
                <h2>Informasi Laporan</h2>
                <p><span class='label'>ID Laporan:</span><span class='value'>{report.Id}</span></p>
                <p><span class='label'>Tanggal Laporan:</span><span class='value'>{report.DateReported:dd MMMM yyyy HH:mm}</span></p>
                <p><span class='label'>Kategori:</span><span class='value'>{report.Category}</span></p>
            </div>

            <div class='section'>
                <h2>Data Pelapor</h2>
                <p><span class='label'>Nama:</span><span class='value'>{anonymousInfo}</span></p>
                <p><span class='label'>Email:</span><span class='value'>{report.Email}</span></p>
                <p><span class='label'>Status:</span><span class='value'>{(report.IsAnonymous ? "Laporan Anonim" : "Laporan Terbuka")}</span></p>
            </div>

            <div class='section'>
                <h2>Data Terlapor</h2>
                <p><span class='label'>Nama Terlapor:</span><span class='value'>{report.ReportedName}</span></p>
                <p><span class='label'>Jabatan:</span><span class='value'>{report.ReportedPosition}</span></p>
                <p><span class='label'>Unit Kerja:</span><span class='value'>{report.ReportedUnit}</span></p>
            </div>

            <div class='section'>
                <h2>Detail Kejadian</h2>
                <p><span class='label'>Waktu Kejadian:</span><span class='value'>{report.IncidentDate:dd MMMM yyyy}</span></p>
                <p><span class='label'>Lokasi Kejadian:</span><span class='value'>{report.IncidentLocation}</span></p>
                <div style='margin-top: 15px;'>
                    <p class='label'>Uraian Kejadian:</p>
                    <div style='background: #f9f9f9; padding: 15px; border-left: 4px solid #dc2626; margin-top: 5px;'>
                        {report.Description.Replace("\n", "<br>")}
                    </div>
                </div>
            </div>

            <div class='section'>
                <h2>Bukti Pendukung</h2>
                {evidenceInfo}
            </div>
        </div>

        <div class='footer'>
            <p>Email ini dikirim otomatis oleh sistem Len Bersih.</p>
            <p>Untuk keamanan dan kerahasiaan, harap segera tindaklanjuti laporan ini sesuai prosedur yang berlaku.</p>
        </div>
    </div>
</body>
</html>";
    }
}