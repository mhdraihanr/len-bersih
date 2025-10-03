using Microsoft.EntityFrameworkCore;
using LenBersih.Api.Data.Entities;

namespace LenBersih.Api.Data;

/// <summary>
/// Database context for Len Bersih application.
/// Manages entity mappings and database operations for whistleblowing system.
/// </summary>
public class LenBersihDbContext : DbContext
{
    public LenBersihDbContext(DbContextOptions<LenBersihDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for all entities

    /// <summary>
    /// Users table - System users with authentication data
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Groups table - User roles (admin, members)
    /// </summary>
    public DbSet<Group> Groups { get; set; }

    /// <summary>
    /// Users_Groups junction table - Many-to-many relationship
    /// </summary>
    public DbSet<UserGroup> UserGroups { get; set; }

    /// <summary>
    /// Pelaporan table - Whistleblowing reports
    /// </summary>
    public DbSet<ReportEntity> Reports { get; set; }

    /// <summary>
    /// Status table - Workflow status lookup
    /// </summary>
    public DbSet<Status> Statuses { get; set; }

    /// <summary>
    /// History_Status table - Audit trail for status changes
    /// </summary>
    public DbSet<HistoryStatus> HistoryStatuses { get; set; }

    /// <summary>
    /// Dokumen table - Evidence files for reports
    /// </summary>
    public DbSet<Dokumen> Dokumen { get; set; }

    /// <summary>
    /// Configure entity relationships, indexes, and constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // USER ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<User>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // Unique indexes (from SQL schema)
            entity.HasIndex(e => e.Email)
                  .IsUnique()
                  .HasDatabaseName("uc_email");

            entity.HasIndex(e => e.ActivationSelector)
                  .IsUnique()
                  .HasDatabaseName("uc_activation_selector");

            entity.HasIndex(e => e.ForgottenPasswordSelector)
                  .IsUnique()
                  .HasDatabaseName("uc_forgotten_password_selector");

            entity.HasIndex(e => e.RememberSelector)
                  .IsUnique()
                  .HasDatabaseName("uc_remember_selector");

            // One-to-many with UserGroups
            entity.HasMany(u => u.UserGroups)
                  .WithOne(ug => ug.User)
                  .HasForeignKey(ug => ug.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // GROUP ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<Group>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // One-to-many with UserGroups
            entity.HasMany(g => g.UserGroups)
                  .WithOne(ug => ug.Group)
                  .HasForeignKey(ug => ug.GroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========================================
        // USERGROUP ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<UserGroup>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.Id);

            // Unique composite index for user_id + group_id
            entity.HasIndex(e => new { e.UserId, e.GroupId })
                  .IsUnique()
                  .HasDatabaseName("uc_users_groups");

            // Foreign key indexes
            entity.HasIndex(e => e.UserId)
                  .HasDatabaseName("fk_users_groups_users1_idx");

            entity.HasIndex(e => e.GroupId)
                  .HasDatabaseName("fk_users_groups_groups1_idx");

            // Configure relationships (already defined in User/Group)
            entity.HasOne(ug => ug.User)
                  .WithMany(u => u.UserGroups)
                  .HasForeignKey(ug => ug.UserId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_users_groups_users1");

            entity.HasOne(ug => ug.Group)
                  .WithMany(g => g.UserGroups)
                  .HasForeignKey(ug => ug.GroupId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_users_groups_groups1");
        });

        // ========================================
        // STATUS ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<Status>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.IdStatus);

            // One-to-many with Reports
            entity.HasMany(s => s.Reports)
                  .WithOne(r => r.Status)
                  .HasForeignKey(r => r.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // One-to-many with HistoryStatus
            entity.HasMany(s => s.HistoryStatusList)
                  .WithOne(h => h.Status)
                  .HasForeignKey(h => h.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        });

        // ========================================
        // REPORTENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<ReportEntity>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.IdPelaporan);

            // Foreign key to Status
            entity.HasOne(r => r.Status)
                  .WithMany(s => s.Reports)
                  .HasForeignKey(r => r.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // One-to-many with Dokumen
            entity.HasMany(r => r.DokumenList)
                  .WithOne(d => d.Report)
                  .HasForeignKey(d => d.IdPelaporan)
                  .OnDelete(DeleteBehavior.Cascade); // Delete documents when report deleted

            // One-to-many with HistoryStatus
            entity.HasMany(r => r.HistoryStatusList)
                  .WithOne(h => h.Report)
                  .HasForeignKey(h => h.IdPelaporan)
                  .OnDelete(DeleteBehavior.Cascade); // Delete history when report deleted
        });

        // ========================================
        // HISTORYSTATUS ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<HistoryStatus>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.IdHistoryStatus);

            // Foreign key to Status
            entity.HasOne(h => h.Status)
                  .WithMany(s => s.HistoryStatusList)
                  .HasForeignKey(h => h.IdStatus)
                  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Foreign key to ReportEntity
            entity.HasOne(h => h.Report)
                  .WithMany(r => r.HistoryStatusList)
                  .HasForeignKey(h => h.IdPelaporan)
                  .OnDelete(DeleteBehavior.Cascade); // Delete history when report deleted
        });

        // ========================================
        // DOKUMEN ENTITY CONFIGURATION
        // ========================================
        modelBuilder.Entity<Dokumen>(entity =>
        {
            // Primary key
            entity.HasKey(e => e.IdDokumen);

            // Foreign key to ReportEntity
            entity.HasOne(d => d.Report)
                  .WithMany(r => r.DokumenList)
                  .HasForeignKey(d => d.IdPelaporan)
                  .OnDelete(DeleteBehavior.Cascade); // Delete document when report deleted
        });
    }
}
