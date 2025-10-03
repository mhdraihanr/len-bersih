using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// Status entity mapping to 'status' table in database.
/// Represents workflow status for reports (e.g., DITERIMA, DITOLAK, DITINDAK LANJUTI).
/// </summary>
[Table("status")]
public class Status
{
    /// <summary>
    /// Primary key - Auto increment integer
    /// </summary>
    [Key]
    [Column("id_status")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdStatus { get; set; }

    /// <summary>
    /// Status name/description (e.g., "Validasi Laporan Tim Sekretariat DITERIMA")
    /// </summary>
    [Column("status")]
    [MaxLength(100)]
    [Required]
    public string StatusName { get; set; } = string.Empty;

    // Navigation properties

    /// <summary>
    /// Reports with this status
    /// </summary>
    public virtual ICollection<ReportEntity> Reports { get; set; } = new List<ReportEntity>();

    /// <summary>
    /// History entries for this status
    /// </summary>
    public virtual ICollection<HistoryStatus> HistoryStatusList { get; set; } = new List<HistoryStatus>();
}
