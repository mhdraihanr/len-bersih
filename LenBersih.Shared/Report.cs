using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace LenBersih.Shared;

public class Report
{
    public int Id { get; set; }

    [MaxLength(150)]
    public string? ReporterName { get; set; }

    public bool IsAnonymous { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ReportedName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ReportedPosition { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ReportedUnit { get; set; } = string.Empty;

    [Required]
    public DateTime IncidentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(200)]
    public string IncidentLocation { get; set; } = string.Empty;

    [Required]
    [MinLength(20)]
    public string Description { get; set; } = string.Empty;

    public string? EvidenceFileName { get; set; }
    public string? EvidenceContentType { get; set; }
    public byte[]? EvidenceData { get; set; }

    [Required]
    [MaxLength(10)]
    public string CaptchaInput { get; set; } = string.Empty;

    public DateTime DateReported { get; set; } = DateTime.UtcNow;
}

public static class ReportMetadata
{
    public const long MaxEvidenceSize = 10 * 1024 * 1024; // 10 MB

    public static readonly IReadOnlyList<string> Categories = new[]
    {
        "Penipuan",
        "Suap",
        "Korupsi",
        "Pencurian",
        "Penggelapan",
        "Gratifikasi",
        "Kecurangan",
        "Konflik Kepentingan",
        "Pelanggaran Hukum",
        "Pelanggaran Peraturan Perusahaan",
        "Penyalahgunaan Jabatan"
    };

    private static readonly HashSet<string> AllowedEvidenceContentTypesSet = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "application/pdf",
        "video/mp4",
        "video/quicktime",
        "video/x-msvideo"
    };

    public static IReadOnlyCollection<string> AllowedEvidenceContentTypes => AllowedEvidenceContentTypesSet;

    private const string CaptchaCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string GenerateCaptchaCode(int length = 5)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var chars = new char[length];
        Span<byte> bytes = stackalloc byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        for (var i = 0; i < length; i++)
        {
            var index = bytes[i] % CaptchaCharacters.Length;
            chars[i] = CaptchaCharacters[index];
        }

        return new string(chars);
    }

    public static bool IsAllowedEvidenceContentType(string? contentType) =>
        !string.IsNullOrWhiteSpace(contentType) && AllowedEvidenceContentTypesSet.Contains(contentType);

    public static bool IsEvidenceSizeValid(long? length) =>
        length is null || length <= MaxEvidenceSize;
}