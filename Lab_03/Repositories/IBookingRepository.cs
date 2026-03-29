using Lab_03.Models;

namespace Lab_03.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking?> GetByDemoPayTokenAsync(string token);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
    }
}
