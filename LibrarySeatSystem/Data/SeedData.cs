using Microsoft.EntityFrameworkCore;
using LibrarySeatSystem.Models.Entities;
using System.Security.Cryptography;
using System.Text;

namespace LibrarySeatSystem.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Seats.Any()) return;

        context.Seats.AddRange(
            new Seat { Name = "A-01", Location = "一楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "A-02", Location = "一楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "A-03", Location = "一楼东侧", HasPower = false, Status = 0 },
            new Seat { Name = "A-04", Location = "一楼西侧", HasPower = false, Status = 0 },
            new Seat { Name = "A-05", Location = "一楼西侧", HasPower = true, Status = 0 },
            new Seat { Name = "B-01", Location = "二楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "B-02", Location = "二楼东侧", HasPower = false, Status = 0 },
            new Seat { Name = "B-03", Location = "二楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "B-04", Location = "二楼西侧", HasPower = false, Status = 0 },
            new Seat { Name = "B-05", Location = "二楼西侧", HasPower = true, Status = 0 },
            new Seat { Name = "C-01", Location = "三楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "C-02", Location = "三楼东侧", HasPower = false, Status = 0 },
            new Seat { Name = "C-03", Location = "三楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "C-04", Location = "三楼靠窗", HasPower = true, Status = 0 },
            new Seat { Name = "C-05", Location = "三楼靠窗", HasPower = true, Status = 0 },
            new Seat { Name = "D-01", Location = "四楼东侧", HasPower = false, Status = 0 },
            new Seat { Name = "D-02", Location = "四楼东侧", HasPower = true, Status = 0 },
            new Seat { Name = "D-03", Location = "四楼西侧", HasPower = false, Status = 0 },
            new Seat { Name = "D-04", Location = "四楼西侧", HasPower = true, Status = 0 },
            new Seat { Name = "D-05", Location = "四楼角落", HasPower = false, Status = 1 }
        );

        context.AdminUsers.AddRange(
            new AdminUser
            {
                Username = "admin",
                Password = ComputeMd5("123456")
            }
        );

        context.SaveChanges();
    }

    private static string ComputeMd5(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
