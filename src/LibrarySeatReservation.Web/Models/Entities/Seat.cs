using System.ComponentModel.DataAnnotations;

namespace LibrarySeatReservation.Web.Models.Entities;

public class Seat
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    public bool HasPower { get; set; }

    public int Status { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
