using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// ReportEntity mapping to 'pelaporan' table in database.
/// Represents whistleblowing reports submitted by users.
/// </summary>
[Table("pelaporan")]
public class ReportEntity
{
    /// <summary>
    /// Primary key - Auto increment integer
    /// </summary>
    [Key]
    [Column("id_pelaporan")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPelaporan { get; set; }

    /// <summary>
    /// Reporter's name (or "Anonim" for anonymous reports)
    /// </summary>
    [Column("nama")]
    [MaxLength(150)]
    [Required]
    public string Nama { get; set; } = string.Empty;

    /// <summary>
    /// Reporter's email address
    /// </summary>
    [Column("email")]
    [MaxLength(150)]
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Report category/type (e.g., "Korupsi", "Penyalahgunaan Wewenang")
    /// </summary>
    [Column("jenis_laporan")]
    [MaxLength(150)]
    [Required]
    public string JenisLaporan { get; set; } = string.Empty;

    /// <summary>
    /// Name of the reported person
    /// </summary>
    [Column("nama_terlapor")]
    [MaxLength(150)]
    [Required]
    public string NamaTerlapor { get; set; } = string.Empty;

    /// <summary>
    /// Position/title of the reported person
    /// </summary>
    [Column("jabatan_terlapor")]
    [MaxLength(150)]
    [Required]
    public string JabatanTerlapor { get; set; } = string.Empty;

    /// <summary>
    /// Work unit/department of the reported person
    /// </summary>
    [Column("unit_kerja_terlapor")]
    [MaxLength(150)]
    [Required]
    public string UnitKerjaTerlapor { get; set; } = string.Empty;

    /// <summary>
    /// Date when the incident occurred
    /// </summary>
    [Column("waktu_kejadian")]
    [Required]
    public DateTime WaktuKejadian { get; set; }

    /// <summary>
    /// Location where the incident occurred
    /// </summary>
    [Column("lokasi_kejadian")]
    [MaxLength(150)]
    [Required]
    public string LokasiKejadian { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the incident
    /// </summary>
    [Column("uraian")]
    [Required]
    public string Uraian { get; set; } = string.Empty;

    /// <summary>
    /// Primary document/evidence file path (legacy field, use Dokumen table instead)
    /// </summary>
    [Column("dokumen")]
    [MaxLength(255)]
    public string Dokumen { get; set; } = string.Empty;

    /// <summary>
    /// Report tracking code (5 characters, e.g., "ABC12")
    /// </summary>
    [Column("kode")]
    [MaxLength(5)]
    [Required]
    public string Kode { get; set; } = string.Empty;

    /// <summary>
    /// Date when report was created/submitted
    /// </summary>
    [Column("created_date")]
    [Required]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Foreign key to status table - Current workflow status
    /// </summary>
    [Column("id_status")]
    [Required]
    public int IdStatus { get; set; }

    /// <summary>
    /// Approval flag (0 = not approved, 1 = approved)
    /// </summary>
    [Column("approve")]
    [Required]
    public int Approve { get; set; }

    /// <summary>
    /// Date when report was approved/reviewed
    /// </summary>
    [Column("approved_date")]
    [Required]
    public DateTime ApprovedDate { get; set; }

    // Navigation properties

    /// <summary>
    /// Navigation to Status entity (current workflow status)
    /// </summary>
    [ForeignKey(nameof(IdStatus))]
    public virtual Status Status { get; set; } = null!;

    /// <summary>
    /// Collection of additional evidence documents
    /// </summary>
    public virtual ICollection<Dokumen> DokumenList { get; set; } = new List<Dokumen>();

    /// <summary>
    /// Collection of status change history (audit trail)
    /// </summary>
    public virtual ICollection<HistoryStatus> HistoryStatusList { get; set; } = new List<HistoryStatus>();

    // Computed properties

    /// <summary>
    /// Computed property: Is report anonymous
    /// </summary>
    [NotMapped]
    public bool IsAnonymous => Nama.Equals("Anonim", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Computed property: Is report approved
    /// </summary>
    [NotMapped]
    public bool IsApproved => Approve == 1;
}
