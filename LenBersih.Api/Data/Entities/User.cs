using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LenBersih.Api.Data.Entities;

/// <summary>
/// User entity mapping to 'users' table in database.
/// Represents system users with authentication and profile information.
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// Primary key - Auto increment unsigned integer
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// IP address of user during registration
    /// </summary>
    [Column("ip_address")]
    [MaxLength(45)]
    [Required]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Username for login (nullable, can use email instead)
    /// </summary>
    [Column("username")]
    [MaxLength(100)]
    public string? Username { get; set; }

    /// <summary>
    /// Hashed password (BCrypt format: $2y$10$...)
    /// </summary>
    [Column("password")]
    [MaxLength(255)]
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Email address (unique, used for login and notifications)
    /// </summary>
    [Column("email")]
    [MaxLength(254)]
    [Required]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Activation selector token (nullable)
    /// </summary>
    [Column("activation_selector")]
    [MaxLength(255)]
    public string? ActivationSelector { get; set; }

    /// <summary>
    /// Activation code for email verification (nullable)
    /// </summary>
    [Column("activation_code")]
    [MaxLength(255)]
    public string? ActivationCode { get; set; }

    /// <summary>
    /// Forgotten password selector token (nullable)
    /// </summary>
    [Column("forgotten_password_selector")]
    [MaxLength(255)]
    public string? ForgottenPasswordSelector { get; set; }

    /// <summary>
    /// Forgotten password reset code (nullable)
    /// </summary>
    [Column("forgotten_password_code")]
    [MaxLength(255)]
    public string? ForgottenPasswordCode { get; set; }

    /// <summary>
    /// Unix timestamp for password reset expiry (nullable)
    /// </summary>
    [Column("forgotten_password_time")]
    public int? ForgottenPasswordTime { get; set; }

    /// <summary>
    /// Remember me selector token (nullable)
    /// </summary>
    [Column("remember_selector")]
    [MaxLength(255)]
    public string? RememberSelector { get; set; }

    /// <summary>
    /// Remember me code (nullable)
    /// </summary>
    [Column("remember_code")]
    [MaxLength(255)]
    public string? RememberCode { get; set; }

    /// <summary>
    /// Unix timestamp for account creation (required)
    /// </summary>
    [Column("created_on")]
    [Required]
    public int CreatedOn { get; set; }

    /// <summary>
    /// Unix timestamp for last login (nullable)
    /// </summary>
    [Column("last_login")]
    public int? LastLogin { get; set; }

    /// <summary>
    /// Active status flag (1 = active, 0 = inactive, nullable)
    /// </summary>
    [Column("active")]
    public byte? Active { get; set; }

    /// <summary>
    /// User's first name (nullable)
    /// </summary>
    [Column("first_name")]
    [MaxLength(50)]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name (nullable)
    /// </summary>
    [Column("last_name")]
    [MaxLength(50)]
    public string? LastName { get; set; }

    /// <summary>
    /// Company or organization name (nullable)
    /// </summary>
    [Column("company")]
    [MaxLength(100)]
    public string? Company { get; set; }

    /// <summary>
    /// Contact phone number (nullable)
    /// </summary>
    [Column("phone")]
    [MaxLength(20)]
    public string? Phone { get; set; }

    // Navigation properties

    /// <summary>
    /// User's group memberships (many-to-many via users_groups table)
    /// </summary>
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    /// <summary>
    /// Computed property: Full name
    /// </summary>
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Computed property: Is user active
    /// </summary>
    [NotMapped]
    public bool IsActive => Active == 1;
}
