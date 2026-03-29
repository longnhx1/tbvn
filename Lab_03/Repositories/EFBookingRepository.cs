using Lab_03.DataAccess;
using Lab_03.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab_03.Repositories
{
    public class EFBookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Tour!)
                .ThenInclude(t => t.Destination)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Tour!)
                .ThenInclude(t => t.Destination)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Tour!)
                .ThenInclude(t => t.Destination)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetByDemoPayTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return await _context.Bookings
                .Include(b => b.Tour!)
                .ThenInclude(t => t.Destination)
                .FirstOrDefaultAsync(b => b.DemoPayToken == token);
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
