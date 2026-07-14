using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibrarySeatSystem.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibrarySeatSystem.Models.Entities;

public class Reservation
{
    [BindNever]
    public int Id { get; set; }

    public int SeatId { get; set; }

    [ForeignKey("SeatId")]
    public Seat? Seat { get; set; }

    [Required]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    public DateTime ReserveDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string TimeSlot { get; set; } = string.Empty;

    public ReservationStatus Status { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
