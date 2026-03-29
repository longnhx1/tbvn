using Lab_03.Models;
using Microsoft.EntityFrameworkCore;
using Lab_03.DataAccess;

namespace Lab_03.Repositories
{
    public class EFDestinationRepository : IDestinationRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDestinationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Destination>> GetAllAsync()
        {
            return await _context.Destinations
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Destination>> GetAllWithToursAsync()
        {
            return await _context.Destinations
                .Include(d => d.Tours)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Destination?> GetByIdAsync(int id)
        {
            return await _context.Destinations.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Destination?> GetByIdWithToursAsync(int id)
        {
            return await _context.Destinations
                .Include(d => d.Tours)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Destination destination)
        {
            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Destination destination)
        {
            _context.Destinations.Update(destination);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination != null)
            {
                _context.Destinations.Remove(destination);
                await _context.SaveChangesAsync();
            }
        }
    }
}
