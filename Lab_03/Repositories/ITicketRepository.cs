using Lab_03.Models;

namespace Lab_03.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetAllAsync();

        Task<Tour?> GetByIdAsync(int id);

        Task AddAsync(Tour tour);

        Task UpdateAsync(Tour tour);

        Task DeleteAsync(int id);

    }
}
