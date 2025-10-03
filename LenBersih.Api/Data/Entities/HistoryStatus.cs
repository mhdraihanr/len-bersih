using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// HistoryStatus entity mapping to 'history_status' table in database.
/// Represents audit trail for status changes on reports.
/// </summary>
[Table("history_status")]
public class HistoryStatus
{
    /// <summary>
    /// Primary key - Auto increment integer
    /// </summary>
    [Key]
    [Column("id_history_status")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdHistoryStatus { get; set; }

    /// <summary>
    /// Foreign key to status table - The status that was applied
    /// </summary>
    [Column("id_status")]
    [Required]
    public int IdStatus { get; set; }

    /// <summary>
    /// Foreign key to pelaporan table - The report being updated
    /// </summary>
    [Column("id_pelaporan")]
    [Required]
    public int IdPelaporan { get; set; }

    /// <summary>
    /// Reason/notes for the status change
    /// </summary>
    [Column("alasan")]
    [Required]
    public string Alasan { get; set; } = string.Empty;

    /// <summary>
    /// Date when status change was recorded
    /// </summary>
    [Column("created_date")]
    [Required]
    public DateTime CreatedDate { get; set; }

    // Navigation properties

    /// <summary>
    /// Navigation to Status entity (the status that was applied)
    /// </summary>
    [ForeignKey(nameof(IdStatus))]
    public virtual Status Status { get; set; } = null!;

    /// <summary>
    /// Navigation to ReportEntity (the report being updated)
    /// </summary>
    [ForeignKey(nameof(IdPelaporan))]
    public virtual ReportEntity Report { get; set; } = null!;
}
