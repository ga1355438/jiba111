using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Services;

public class SeatService : ISeatService
{
    private readonly DataAccess.ISeatRepository _seatRepository;

    public SeatService(DataAccess.ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public Task<List<Seat>> GetAllSeatsAsync() => _seatRepository.GetAllAsync();
    public Task<Seat?> GetSeatByIdAsync(int id) => _seatRepository.GetByIdAsync(id);
    public Task<bool> CreateSeatAsync(Seat seat) { _seatRepository.AddAsync(seat); return Task.FromResult(true); }
    public Task<bool> UpdateSeatAsync(Seat seat) { _seatRepository.UpdateAsync(seat); return Task.FromResult(true); }
    public Task<bool> DeleteSeatAsync(int id) { _seatRepository.DeleteAsync(id); return Task.FromResult(true); }
}
