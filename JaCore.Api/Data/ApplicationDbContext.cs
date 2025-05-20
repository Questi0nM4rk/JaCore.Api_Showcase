using JaCore.Api.Entities.Auth;
using JaCore.Api.Entities.Device;
using JaCore.Api.Entities.Identity;
using JaCore.Api.Entities.Interfaces;
using JaCore.Api.Entities.Template;
using JaCore.Api.Entities.Work;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace JaCore.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    // Device Domain DbSets
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<MetConfirmation> MetConfirmations { get; set; } = null!;
    public DbSet<ServiceEntity> ServiceEntities { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<DeviceCard> DeviceCards { get; set; } = null!;
    public DbSet<DeviceOperation> DeviceOperations { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;

    // Template Domain DbSets
    public DbSet<Template> Templates { get; set; } = null!;
    public DbSet<TemplateOperation> TemplateOperations { get; set; } = null!;
    public DbSet<TemplateParameter> TemplateParameters { get; set; } = null!;

    // Work Domain DbSets
    public DbSet<Work> Works { get; set; } = null!;
    public DbSet<WorkOperation> WorkOperations { get; set; } = null!;
    public DbSet<WorkStep> WorkSteps { get; set; } = null!;
    public DbSet<WorkResult> WorkResults { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Important for Identity schema

        // Configure RefreshToken entity
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.TokenHash).IsRequired();
            entity.Property(rt => rt.ExpiryDate).IsRequired();

            // Relationship: One User can have potentially many RefreshTokens over time
            entity.HasOne(rt => rt.User)
                  .WithMany() // No navigation property on User needed for this side
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // If user is deleted, cascade delete their refresh tokens

            // Optional index for performance
            entity.HasIndex(rt => rt.UserId);
        });

        // Device Domain Configurations
        ConfigureDeviceDomain(builder);

        // Template Domain Configurations (just TemplateUIElem for now)
        ConfigureTemplateDomainMinimal(builder);
        // Full Template domain
        ConfigureTemplateDomain(builder);

        // Work Domain Configurations
        ConfigureWorkDomain(builder);
    }

    private void ConfigureDeviceDomain(ModelBuilder builder)
    {
        // Location
        builder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.ToTable(tb => tb.HasCheckConstraint("Location_IsRemoved_CHK",
                "((\"IsRemoved\" = FALSE) OR (\"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });

        // MetConfirmation
        builder.Entity<MetConfirmation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Lvl1).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Lvl2).HasMaxLength(100);
            entity.Property(e => e.Lvl3).HasMaxLength(100);
            entity.Property(e => e.Lvl4).HasMaxLength(100);
            entity.ToTable(tb => tb.HasCheckConstraint("MetConfirmation_IsRemoved_CHK",
                "((\"IsRemoved\" = FALSE) OR (\"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });

        // ServiceEntity (table named "Service" in DB)
        builder.Entity<ServiceEntity>(entity =>
        {
            entity.ToTable("Service"); // Ensure table name matches schema
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Contact).HasMaxLength(100);
            entity.ToTable(tb => tb.HasCheckConstraint("Service_IsRemoved_CHK",
                "((\"IsRemoved\" = FALSE) OR (\"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });

        // Supplier
        builder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Contact).HasMaxLength(100);
            entity.ToTable(tb => tb.HasCheckConstraint("Supplier_IsRemoved_CHK",
                "((\"IsRemoved\" = FALSE) OR (\"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });

        // Device
        builder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(d => d.Location).WithMany(l => l.Devices).HasForeignKey(d => d.LocationId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.LocationId).HasDatabaseName("Device_idx_LocationIndex");
            entity.ToTable(tb => tb.HasCheckConstraint("Device_IsRemoved_CHK",
                "(( \"IsRemoved\" = FALSE) OR ( \"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
            entity.ToTable(tb => tb.HasCheckConstraint("Device_IsDisabled_CHK",
                "(((\"IsDisabled\" = FALSE)OR (\"IsDisabled\" = TRUE AND \"DisabledBy\" IS NOT NULL AND \"DisabledAt\" IS NOT NULL)))"));
            // One-to-one with DeviceCard: DeviceCard is dependent, Device is principal.
            // EF Core convention usually handles this if DeviceCard has a required FK to Device and unique index.
            // Device.DeviceCard navigation property is nullable, DeviceCard.Device is not.
        });

        // DeviceCard
        builder.Entity<DeviceCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(20);
            entity.HasOne(dc => dc.Device).WithOne(d => d.DeviceCard).HasForeignKey<DeviceCard>(dc => dc.DeviceId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(dc => dc.DeviceId).IsUnique().HasDatabaseName("DeviceCard_DeviceId_Unique");

            entity.HasOne(dc => dc.Supplier).WithMany(s => s.DeviceCards).HasForeignKey(dc => dc.SupplierId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(dc => dc.Service).WithMany(s => s.DeviceCards).HasForeignKey(dc => dc.ServiceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(dc => dc.MetConfirmation).WithMany(mc => mc.DeviceCards).HasForeignKey(dc => dc.MetConfirmationId).OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SupplierId).HasDatabaseName("DeviceCard_idx_SupplierIndex");
            entity.HasIndex(e => e.ServiceId).HasDatabaseName("DeviceCard_idx_ServiceIndex");
            entity.HasIndex(e => e.MetConfirmationId).HasDatabaseName("DeviceCard_idx_MetConfIndex");

            entity.ToTable(tb => tb.HasCheckConstraint("DeviceCard_IsRemoved_CHK",
                "(( \"IsRemoved\" = FALSE) OR ( \"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
            
            // Many-to-many DeviceCard <-> DeviceOperation
            entity.HasMany(dc => dc.DeviceOperations)
                  .WithMany(dop => dop.DeviceCards)
                  .UsingEntity<Dictionary<string, object>>(
                        "DeviceCard_DeviceOperation", // Explicit join table name
                        j => j.HasOne<DeviceOperation>().WithMany().HasForeignKey("DeviceOperationId"),
                        j => j.HasOne<DeviceCard>().WithMany().HasForeignKey("DeviceCardId"),
                        j =>
                        {
                            j.HasKey("DeviceCardId", "DeviceOperationId");
                            j.ToTable("DeviceCard_DeviceOperation");
                            j.HasIndex("DeviceCardId").HasDatabaseName("DeviceCard_DeviceOperation_idx_Card");
                            j.HasIndex("DeviceOperationId").HasDatabaseName("DeviceCard_DeviceOperation_idx_Op");
                        });
        });

        // DeviceOperation
        builder.Entity<DeviceOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Label).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Value).HasColumnType("decimal(5,5)");
            entity.Property(e => e.Unit).HasMaxLength(10);
            // Foreign key to TemplateUIElem
            entity.HasOne(dop => dop.TemplateUIElem)
                  .WithMany() // Assuming TemplateUIElem can be used by many DeviceOperations/TemplateOperations
                  .HasForeignKey(dop => dop.TemplateUIElemId)
                  .OnDelete(DeleteBehavior.Restrict); // Or SetNull if UIElem can be nullable and you want to keep the op
            entity.HasIndex(e => e.TemplateUIElemId).HasDatabaseName("DeviceOperation_TemplateUIElemIdIndex");

            entity.ToTable(tb => tb.HasCheckConstraint("DeviceOperation_ValueUnit_CHK",
                "(((\"Value\" IS NULL AND \"Unit\" IS NULL) OR (\"Value\" IS NOT NULL AND \"Unit\" IS NOT NULL)))"));
            entity.ToTable(tb => tb.HasCheckConstraint("DeviceOperation_IsRemoved_CHK",
                "((\"IsRemoved\" = FALSE) OR (\"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });

        // Event
        builder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.HasOne(e => e.DeviceCard).WithMany(dc => dc.Events).HasForeignKey(e => e.DeviceCardId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.DeviceCardId).HasDatabaseName("Event_idx_DeviceCardIndex");
            entity.ToTable(tb => tb.HasCheckConstraint("Event_IsRemoved_CHK",
                "(( \"IsRemoved\" = FALSE) OR ( \"IsRemoved\" = TRUE AND \"RemovedBy\" IS NOT NULL AND \"RemovedAt\" IS NOT NULL))"));
        });
    }

    private void ConfigureTemplateDomainMinimal(ModelBuilder builder)
    {
        // TemplateUIElem
        builder.Entity<TemplateUIElem>(entity =>
        {
            entity.ToTable("TemplateUIElem");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(20);
            // ElemType int NOT NULL - will be mapped by property type
        });
    }

    // Full Template domain configuration
    private void ConfigureTemplateDomain(ModelBuilder builder)
    {
        builder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        builder.Entity<TemplateOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        builder.Entity<TemplateParameter>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }

    // Work domain configuration
    private void ConfigureWorkDomain(ModelBuilder builder)
    {
        builder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        builder.Entity<WorkOperation>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        builder.Entity<WorkStep>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        builder.Entity<WorkResult>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetAuditProperties();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetAuditProperties();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetAuditProperties()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditable || e.Entity is ISoftDeletable || e.Entity is IDisableable)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        var currentUserId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM"; // Default to SYSTEM if no user
        var now = DateTime.UtcNow;

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Deleted && entityEntry.Entity is ISoftDeletable softDeletableEntity)
            {
                entityEntry.State = EntityState.Modified; // Prevent hard delete
                softDeletableEntity.IsRemoved = true;
                softDeletableEntity.RemovedAt = now;
                softDeletableEntity.RemovedBy = currentUserId;
            }

            if (entityEntry.Entity is IAuditable auditableEntity)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    auditableEntity.CreatedAt = now;
                    auditableEntity.CreatedBy = currentUserId;
                    auditableEntity.ModifiedAt = now;
                    auditableEntity.ModifiedBy = currentUserId;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    // If IsRemoved was just set by soft delete logic, ModifiedAt/By was already set by that block
                    if (!(entityEntry.Entity is ISoftDeletable sd && sd.IsRemoved && sd.RemovedAt == now))
                    {
                         // Also check for IDisableable, if IsDisabled was just set, it handles its own DisabledAt/By
                        if (!(entityEntry.Entity is IDisableable dis && dis.IsDisabled && dis.DisabledAt == now && auditableEntity.ModifiedAt == now))
                        {
                            auditableEntity.ModifiedAt = now;
                            auditableEntity.ModifiedBy = currentUserId;
                        }
                    }
                }
            }

            // Handle IDisableable - this can be set independently of Added/Modified state for IAuditable
            // e.g. when an entity is disabled, IsDisabled changes, and we need to record DisabledAt/By
            if (entityEntry.Entity is IDisableable disableableEntity)
            {
                // Check if IsDisabled property was actually modified to true
                var originalIsDisabled = entityEntry.OriginalValues.GetValue<bool>(nameof(IDisableable.IsDisabled));
                if (disableableEntity.IsDisabled && !originalIsDisabled)
                {
                    disableableEntity.DisabledAt = now;
                    disableableEntity.DisabledBy = currentUserId;
                    // If auditable, also update ModifiedAt/By
                    if (entityEntry.Entity is IAuditable auditableForDisable) 
                    {
                        auditableForDisable.ModifiedAt = now;
                        auditableForDisable.ModifiedBy = currentUserId;
                    }
                }
                // If entity is re-enabled
                else if (!disableableEntity.IsDisabled && originalIsDisabled)
                {
                    disableableEntity.DisabledAt = null;
                    disableableEntity.DisabledBy = null;
                    if (entityEntry.Entity is IAuditable auditableForEnable) 
                    {
                        auditableForEnable.ModifiedAt = now;
                        auditableForEnable.ModifiedBy = currentUserId;
                    }
                }
            }
        }
    }
}
