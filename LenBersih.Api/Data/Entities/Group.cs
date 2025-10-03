using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// Group entity mapping to 'groups' table in database.
/// Represents user roles (admin, members).
/// </summary>
[Table("groups")]
public class Group
{
    /// <summary>
    /// Primary key - Auto increment mediumint unsigned
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Group name (e.g., "admin", "members")
    /// </summary>
    [Column("name")]
    [MaxLength(20)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Group description (e.g., "Administrator", "General User")
    /// </summary>
    [Column("description")]
    [MaxLength(100)]
    [Required]
    public string Description { get; set; } = string.Empty;

    // Navigation properties

    /// <summary>
    /// Users in this group (many-to-many via users_groups table)
    /// </summary>
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
