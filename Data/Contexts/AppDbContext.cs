using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserEntity>(options)
{
    public virtual DbSet<ClientEntity> Clients { get; set; }
    public virtual DbSet<StatusEntity> Statuses { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<AddressEntity> Addresses { get; set; }
    public virtual DbSet<ProjectMemberEntity> Members { get; set; }
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<NotificationDismissedEntity> DismissedNotifications { get; set; }
    public virtual DbSet<NotificationTypeEntity> NotificationTypes { get; set; }
    public virtual DbSet<NotificationTargetGroupEntity> NotificationTargetGroups { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<ProjectMemberEntity>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserEntity>()
            .HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<UserEntity>(u => u.AddressId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
