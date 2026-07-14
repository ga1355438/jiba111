using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SeatId, e.ReserveDate, e.TimeSlot }).IsUnique();
            entity.HasIndex(e => e.UserName);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ReserveDate);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.TimeSlot).HasMaxLength(20);
            entity.HasOne(e => e.Seat).WithMany().HasForeignKey(e => e.SeatId);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(100);
        });
    }
}
