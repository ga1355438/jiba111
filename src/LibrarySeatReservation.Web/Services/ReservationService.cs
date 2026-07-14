using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.DataAccess;

namespace LibrarySeatReservation.Web.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public Task<List<Reservation>> GetReservationsByUserAsync(string userName) 
        => _reservationRepository.GetByUserAsync(userName);

    public Task<List<Reservation>> GetAllReservationsAsync() 
        => _reservationRepository.GetAllAsync();

    public async Task<bool> CreateReservationAsync(Reservation reservation)
    {
        bool conflict = await _reservationRepository.HasConflictAsync(
            reservation.SeatId, reservation.ReserveDate, reservation.TimeSlot);
        if (conflict) return false;

        await _reservationRepository.AddAsync(reservation);
        return true;
    }

    public async Task<bool> CancelReservationAsync(int id, string userName)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation == null || reservation.UserName != userName || reservation.Status != 0)
            return false;

        reservation.Status = 2;
        await _reservationRepository.UpdateAsync(reservation);
        return true;
    }
}
