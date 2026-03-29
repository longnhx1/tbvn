using Lab_03.Models;
using Microsoft.EntityFrameworkCore;
using Lab_03.DataAccess;

namespace Lab_03.Repositories
{
    public class EFTourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;
        public EFTourRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Tour>> GetAllAsync()
        {
            // return await _context.Tickets.ToListAsync();
            return await _context.Tours
            .Include(p => p.Destination) // Include thông tin về điểm đến
            .ToListAsync();
        }
        public async Task<Tour?> GetByIdAsync(int id)
        {
            return await _context.Tours
                .Include(p => p.Destination)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Tour tour)
        {
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return;
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
    }
}
