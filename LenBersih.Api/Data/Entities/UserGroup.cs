using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// UserGroup junction table entity mapping to 'users_groups' table.
/// Represents many-to-many relationship between Users and Groups.
/// </summary>
[Table("users_groups")]
public class UserGroup
{
    /// <summary>
    /// Primary key - Auto increment unsigned integer
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to users table
    /// </summary>
    [Column("user_id")]
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key to groups table
    /// </summary>
    [Column("group_id")]
    [Required]
    public int GroupId { get; set; }

    // Navigation properties

    /// <summary>
    /// Navigation to User entity
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Navigation to Group entity
    /// </summary>
    [ForeignKey(nameof(GroupId))]
    public virtual Group Group { get; set; } = null!;
}
