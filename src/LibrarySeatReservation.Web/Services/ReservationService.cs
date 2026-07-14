using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public class ReservationService : IReservationService
{
    private readonly DataAccess.IReservationRepository _reservationRepository;

    public ReservationService(DataAccess.IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public Task<List<Reservation>> GetReservationsByUserAsync(string userName) => _reservationRepository.GetByUserAsync(userName);
    public Task<List<Reservation>> GetAllReservationsAsync() => _reservationRepository.GetAllAsync();
    public Task<bool> CreateReservationAsync(Reservation reservation) { _reservationRepository.AddAsync(reservation); return Task.FromResult(true); }
    public Task<bool> CancelReservationAsync(int id, string userName) { return Task.FromResult(true); }
}
