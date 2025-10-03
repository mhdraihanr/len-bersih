using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// Dokumen entity mapping to 'dokumen' table in database.
/// Represents evidence files attached to reports (supports multiple files per report).
/// </summary>
[Table("dokumen")]
public class Dokumen
{
    /// <summary>
    /// Primary key - Auto increment integer
    /// </summary>
    [Key]
    [Column("id_dokumen")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdDokumen { get; set; }

    /// <summary>
    /// File path or filename of the evidence document
    /// </summary>
    [Column("dokumen")]
    [MaxLength(255)]
    [Required]
    public string DokumenPath { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to pelaporan table - The report this document belongs to
    /// </summary>
    [Column("id_pelaporan")]
    [Required]
    public int IdPelaporan { get; set; }

    // Navigation properties

    /// <summary>
    /// Navigation to ReportEntity (parent report)
    /// </summary>
    [ForeignKey(nameof(IdPelaporan))]
    public virtual ReportEntity Report { get; set; } = null!;
}
