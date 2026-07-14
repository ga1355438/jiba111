using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Models.Entities;

namespace LibrarySeatSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seat>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).HasMaxLength(50);
            e.Property(s => s.Location).HasMaxLength(100);
            e.HasIndex(s => s.Name).IsUnique();
            e.HasIndex(s => s.Status);
        });

        modelBuilder.Entity<Reservation>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.UserName).HasMaxLength(50);
            e.Property(r => r.TimeSlot).HasMaxLength(20);
            e.HasOne(r => r.Seat).WithMany().HasForeignKey(r => r.SeatId);
            e.HasIndex(r => new { r.SeatId, r.ReserveDate, r.TimeSlot }).IsUnique();
            e.HasIndex(r => r.UserName);
            e.HasIndex(r => r.Status);
            e.HasIndex(r => r.ReserveDate);
        });

        modelBuilder.Entity<AdminUser>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Username).HasMaxLength(50);
            e.Property(a => a.Password).HasMaxLength(100);
            e.HasIndex(a => a.Username).IsUnique();
        });
    }
}
