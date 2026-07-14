using System.ComponentModel.DataAnnotations;

namespace LibrarySeatReservation.Web.Models.Entities;

public class Reservation
{
    public int Id { get; set; }

    public int SeatId { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    public DateTime ReserveDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string TimeSlot { get; set; } = string.Empty;

    public int Status { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Seat? Seat { get; set; }
}
