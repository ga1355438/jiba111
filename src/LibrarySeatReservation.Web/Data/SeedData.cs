using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Helpers;

namespace LibrarySeatReservation.Web.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Seats.Any()) return;

        context.Seats.AddRange(
            new Seat { Name = "A-01", Location = "三楼东侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "A-02", Location = "三楼东侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "A-03", Location = "三楼东侧靠窗", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "A-04", Location = "三楼东侧过道", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "A-05", Location = "三楼东侧过道", HasPower = false, Status = 1, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "B-01", Location = "三楼西侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "B-02", Location = "三楼西侧靠窗", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "B-03", Location = "三楼西侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "B-04", Location = "三楼西侧过道", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "B-05", Location = "三楼西侧过道", HasPower = true, Status = 1, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "C-01", Location = "二楼东侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "C-02", Location = "二楼东侧靠窗", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "C-03", Location = "二楼东侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "C-04", Location = "二楼东侧过道", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "C-05", Location = "二楼东侧过道", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "D-01", Location = "二楼西侧靠窗", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "D-02", Location = "二楼西侧靠窗", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "D-03", Location = "二楼西侧靠窗", HasPower = false, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "D-04", Location = "二楼西侧过道", HasPower = true, Status = 0, CreatedAt = DateTime.UtcNow },
            new Seat { Name = "D-05", Location = "二楼西侧过道", HasPower = false, Status = 1, CreatedAt = DateTime.UtcNow }
        );

        if (!context.AdminUsers.Any())
        {
            context.AdminUsers.Add(new AdminUser
            {
                Username = "admin",
                Password = PasswordHelper.HashPassword("123456"),
                CreatedAt = DateTime.UtcNow
            });
        }

        context.SaveChanges();
    }
}
